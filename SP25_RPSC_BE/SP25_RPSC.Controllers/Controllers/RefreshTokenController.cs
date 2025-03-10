using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.RefreshTokenModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.RefeshTokenService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokenController(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenReqModel refreshTokenReqModel)
        {
            var result = await _refreshTokenService.RefreshToken(refreshTokenReqModel);


            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Refresh token successfully",
                Data = result
            };

            return StatusCode(response.Code, response);
        }
    }
}
