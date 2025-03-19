using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
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
            int pageIndex, int pageSize,
            string landlordId, string searchQuery = null)
        {
            var rooms = await _roomService.GetRequiresRoomRentalByLandlordId(landlordId, searchQuery, pageIndex, pageSize);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get room rental requests successfully",
                Data = rooms
            };

            return StatusCode(response.Code, response);
        }

    }
}
