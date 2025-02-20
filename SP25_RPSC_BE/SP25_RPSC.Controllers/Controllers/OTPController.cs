using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.OTPModel;
using SP25_RPSC.Services.OTPService;

namespace EXE201_Workshopista.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private readonly IOTPService _oTPService;

        public OTPController(IOTPService oTPService)
        {
            _oTPService = oTPService;
        }

        [HttpPut("verify-email")]
        public async Task<IActionResult> VerifyEmail(OTPVerifyRequestModel model)
        {
            await _oTPService.VerifyOTPToActivateAccount(model);
            return Ok("Activate account successfully");
        }
    }
}
