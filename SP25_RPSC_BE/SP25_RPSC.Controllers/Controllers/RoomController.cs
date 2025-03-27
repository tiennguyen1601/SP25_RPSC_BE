using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.Models.RoomModel.RequestModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Services.Service.RoomServices;
using SP25_RPSC.Services.Service.RoomTypeService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomServices _roomService;

        public RoomController(IRoomServices roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [Route("Get-Requires-Room-Rental")]
        public async Task<ActionResult> GetRequiresRoomRentalByLandlord(
            int pageIndex, int pageSize, string searchQuery = null)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var rooms = await _roomService.GetRequiresRoomRentalByLandlordId(token, searchQuery, pageIndex, pageSize);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get room rental requests successfully",
                Data = rooms
            };

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Route("Create-Room")]
        public async Task<ActionResult> CreateRoom(RoomCreateRequestModel model)
        {
            //var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value.ToString();
            var result = await _roomService.CreateRoom(model);

            if (result)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Room created successfully"
                });
            }

            return BadRequest(new { Message = "Room cannot be created" });
        }

    }
}
