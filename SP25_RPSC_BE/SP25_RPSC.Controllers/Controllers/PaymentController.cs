using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Org.BouncyCastle.Asn1.Ocsp;
using SP25_RPSC.Data.Models.PackageModel.Request;
using SP25_RPSC.Data.Models.PayOSModel;
using SP25_RPSC.Services.Service.PaymentService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        [HttpPost("package")]
        //[Authorize(Roles = "Landlord")]
        public async Task<IActionResult> CreateRequestUrl([FromForm] PaymentPackageRequestDTO request)
        {
            // call payment service
            var response = await _paymentService.CreatePaymentPackageRequest(request, HttpContext);

            return Created("", response);
        }

        [HttpPost("package/extend")]
        [Authorize(Roles = "REPRESENTATIVE")]
        public async Task<IActionResult> CreateExtendRequestUrl([FromForm] PaymentExtendPackageRequestDTO request)
        {
            // call payment service
            var response = await _paymentService.CreateExtendPackageRequest(request, HttpContext);

            return Ok(response);
        }

        [HttpPost("package/handle-response")]
        //[Authorize(Roles = "Landlord")]
        public async Task<IActionResult> HandleResponse([FromBody] PaymentPackageResponseDTO responseInfo)
        {
            if (long.TryParse(responseInfo.TransactionNumber, out long transactionNumber))
            {
                PayOS payOS = new PayOS(_config["PayOS:ClientID"], _config["PayOS:ApiKey"], _config["PayOS:ChecksumKey"]);

                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(transactionNumber);

                var response = await _paymentService.HandlePaymentPackageResponse(responseInfo);

                return Ok(new
                {
                    PaymentResponse = response,
                    PaymentLinkInformation = paymentLinkInformation
                });
            }
            else
            {
                // If the conversion fails, return an error response
                return BadRequest("Invalid transaction number.");
            }
        }
    }
}
