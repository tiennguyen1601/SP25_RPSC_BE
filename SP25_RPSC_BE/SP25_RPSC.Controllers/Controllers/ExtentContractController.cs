using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ExtendContract.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.ExtendContractService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestExtendContractController : ControllerBase
    {
        private readonly IExtendContractService _requestExtendContractService;

        public RequestExtendContractController(IExtendContractService requestExtendContractService)
        {
            _requestExtendContractService = requestExtendContractService;
        }

        [HttpPost]
        [Route("Create-ExtendContract")]
        public async Task<ActionResult> CreateRequestExtendContract(CreateRequestExtendContract model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                await _requestExtendContractService.CreateRequestExtendContractAsync(model, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Request extend contract created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Fail to request extend contract.",
                    Data = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("Reject-ExtendContract")]
        public async Task<ActionResult> RejectExtendContract([FromBody] RejectExtendContractRequest model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                await _requestExtendContractService.RejectExtendContractAsync(model.RequestId, model.MessageLandlord, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Request extend contract rejected successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Fail to reject request extend contract.",
                    Data = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("Approve-ExtendContract")]
        public async Task<ActionResult> ApproveExtendContract([FromBody] RejectExtendContractRequest model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                await _requestExtendContractService.ApproveExtendContractAsync(model.RequestId, model.MessageLandlord, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Request extend contract approved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Fail to approve request extend contract.",
                    Data = ex.Message
                });
            }
        }
        [HttpGet("View-All-Request-by-landlord")]
        public async Task<IActionResult> ViewAllRequestExtendContract(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _requestExtendContractService.ViewAllRequestExtendContractAsync(token, pageIndex, pageSize);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Error retrieving extend contract requests",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("View-Detail-Request-By-Contract")]
        public async Task<IActionResult> ViewDetailRequestExtendContractByContractId([FromQuery] string contractId)
        {
            try
            {
                var result = await _requestExtendContractService.ViewDetailRequestExtendContractByContractId(contractId);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Successfully retrieved contract detail and extend requests.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Error retrieving contract details.",
                    Data = ex.Message
                });
            }
        }
        [HttpGet("View-All-Request-by-Customer")]
        public async Task<IActionResult> ViewAllRequestExtendContractByCustomer(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _requestExtendContractService.ViewAllRequestExtendContractByCustomerAsync(token, pageIndex, pageSize);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Success",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Error retrieving extend contract requests",
                    Data = ex.Message
                });
            }
        }

    }
}
