﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.PackageModel.Request;
using SP25_RPSC.Services.Service.PaymentService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("package")]
        public async Task<IActionResult> CreateRequestUrl([FromForm] PaymentPackageRequestDTO request)
        {
            // call payment service
            var response = await _paymentService.CreatePaymentPackageRequest(request, HttpContext);

            return Created("", response);
        }
    }
}
