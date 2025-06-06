﻿using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.OTPModel;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
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


        [HttpPost("create-service-detail")]
        public async Task<IActionResult> CreateServiceDetail([FromBody] ServiceDetailCreateRequestModel model)
        {
            if (model == null)
            {
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request data."
                });
            }

            await _packageService.CreateServiceDetail(model);

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Service detail created successfully."
            });
        }


        [HttpGet("get-service-package-by-landlord")]
        public async Task<ActionResult> GetServicePackageforLandlord()
        {
            var result = await _packageService.GetServicePackageForLanlord();

            return Ok(result);
        }
        [HttpPut("update-price/{priceId}")]
        public async Task<IActionResult> UpdatePrice(string priceId, [FromBody] UpdatePriceRequest request)
        {
            if (request == null || request.NewPrice <= 0)
            {
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid price value."
                });
            }

            await _packageService.UpdatePriceAndServiceDetail(priceId, request.NewPrice,request.NewName ,request.NewDuration, request.NewDescription, request.NewStatus);

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Price package updated successfully."
            });
        }
        [HttpPut("update-service-package/{packageId}")]
        public async Task<IActionResult> UpdateServicePackage(string packageId, [FromBody] UpdateServicePackageRequest model)
        {
            if (model == null)
            {
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request data."
                });
            }

            await _packageService.UpdateServicePackage(
                packageId,
                model.NewType,
                model.NewHighLightTime,
                model.NewPriorityTime,
                model.NewMaxPost,
                model.NewLabel,
                model.NewStatus
            );

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Service package updated successfully."
            });
        }



    }
}
