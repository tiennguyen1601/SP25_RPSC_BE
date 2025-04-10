using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.Models.RoomRentRequestModel;
using SP25_RPSC.Services.Service.CustomerRentRoomDetailRequestServices;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRequestRentController : ControllerBase
    {
        private readonly ICustomerRentRoomDetailRequestService _customerRentRoomDetailRequestService;

        public CustomerRequestRentController(ICustomerRentRoomDetailRequestService customerRentRoomDetailRequestService)
        {
            _customerRentRoomDetailRequestService = customerRentRoomDetailRequestService;
        }

        [HttpPost("room-rent-request")]
        public async Task<IActionResult> RoomRentRequest([FromBody] RoomRentRequestCreateModel request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _customerRentRoomDetailRequestService.CreateRentRequest(request, token);
            if (result != null)
            {
                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Send request room  successfully.",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            else
            {
                ResultModel response = new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to send request room .",
                };
                return StatusCode(response.Code, response);
            }

          
         }
        [HttpGet("room-rent-request")]
        public async Task<IActionResult> GetRoomRentRequest([FromQuery] string ?roomId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _customerRentRoomDetailRequestService.GetRoomRentRequestsByRoomId(roomId, token);
            if (result != null)
            {
                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get request room successfully.",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            else
            {
                ResultModel response = new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to Get request room .",
                };
                return StatusCode(response.Code, response);
            }


        }
    }
}
