using SP25_RPSC.Data.Entities;

namespace SP25_RPSC.Services.Service.JWTService
{
    public interface IJWTService
    {
        string GenerateJWT(User user);
        string GenerateRefreshToken();
        string decodeToken(string jwtToken, string nameClaim);
    }
}
