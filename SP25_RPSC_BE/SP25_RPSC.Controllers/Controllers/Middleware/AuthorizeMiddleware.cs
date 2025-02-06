using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Repositories.UserRepository;
using SP25_RPSC.Services.Utils.CustomException;
using System.Net;
using System.Security.Claims;

namespace SP25_RPSC.Controllers.Controllers.Middleware
{
    public class AuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepository)
        {
            try
            {
                var requestPath = context.Request.Path;

                if (requestPath.StartsWithSegments("/api/authentication"))
                {
                    await _next.Invoke(context);
                    return;
                }

                var userIdentity = context.User.Identity as ClaimsIdentity;
                if (!userIdentity.IsAuthenticated)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                var user = await userRepository.Get((int.Parse(userIdentity.FindFirst("userid").Value)));

                if (user != null)
                {
                    if (user.Status.Equals(StatusEnums.Inactive.ToString()))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }
                else
                {
                    throw new ApiException(HttpStatusCode.NotFound, "User not found");
                }

                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.ToString());
            }

        }
    }
}
