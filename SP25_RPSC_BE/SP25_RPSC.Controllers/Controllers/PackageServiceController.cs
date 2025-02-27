using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.OTPModel;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Services.Service.OTPService;
using SP25_RPSC.Services.Service.PackageService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageServiceController : Controller
    {
        private readonly IPackageService _packageService;

        public PackageServiceController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost("Create-Service")]
        public async Task<IActionResult> CreateService(PackageCreateRequestModel model)
        {
            await _packageService.CreatePackage(model);
            return Ok("Create Service successfully");
        }
    }
}
