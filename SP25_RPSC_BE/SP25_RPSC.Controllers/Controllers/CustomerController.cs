
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.CustomerService;
using SP25_RPSC.Services.Service.RoomStayService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        [HttpPost("room-sharing-request")]
        public async Task<IActionResult> RoomSharingRequest([FromBody] RoomSharingReq request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _customerService.SendRequestRoomSharing(token, request);
            if (result) {
                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Send request room sharing successfully",
                };
                return StatusCode(response.Code, response);
            }
            else {
                ResultModel response = new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to send request room sharing",
                };
                return StatusCode(response.Code, response);
            }
        }
    }
}
