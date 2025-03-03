using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.OTPModel;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.OTPService;
using SP25_RPSC.Services.Service.PackageService;
using System.Net;

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
        [HttpGet]
        public async Task<ActionResult> GetAllServicePackage()
        {
            var result = await _packageService.GetAllServicePackage();
            
            return Ok(result);
        }

        [HttpGet("get-service-details/{packageId}")]
        public async Task<IActionResult> GetServiceDetailsByPackageId(string packageId)
        {
            var result = await _packageService.GetServiceDetailsByPackageId(packageId);
            return Ok(result); 
        }


        [HttpPost("Create-Service")]
        public async Task<IActionResult> CreateService(PackageCreateRequestModel model)
        {
            await _packageService.CreatePackage(model);
            return Ok("Create Service successfully");
        }

    }
}
