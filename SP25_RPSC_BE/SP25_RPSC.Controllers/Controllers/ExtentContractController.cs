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
                var result = await _requestExtendContractService.CreateRequestExtendContractAsync(model, token);

                if (result.ToString() != "")
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Request extend contract created successfully"
                    });
                }

                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Request extend contract cannot be created"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Data = ex.Message
                });
            }
        }
    }
}
