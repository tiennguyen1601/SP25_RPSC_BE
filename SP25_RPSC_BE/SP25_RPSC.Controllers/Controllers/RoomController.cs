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
        [HttpGet]
        [Route("Get-Room-Counts")]
        public async Task<ActionResult> GetRoomCountsByLandlord()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var roomCounts = await _roomService.GetRoomCountsByLandlordId(token);

            if (roomCounts == null)
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No room count data found"
                });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Room counts retrieved successfully",
                Data = roomCounts
            });
        }
        [HttpGet]
        [Route("Get-Rooms-By-RoomTypeId")]
        public async Task<ActionResult> GetRoomByRoomTypeId(
            string roomTypeId,
            int pageIndex,
            int pageSize,
            string searchQuery = "",
            string status = null)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var rooms = await _roomService.GetRoomByRoomTypeId(roomTypeId, pageIndex, pageSize, searchQuery, status);

            if (rooms == null)
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No rooms found for this RoomTypeId"
                });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Rooms retrieved successfully",
                Data = rooms
            });
        }
        [HttpGet]
        [Route("Get-Room-Detail-By-RoomId")]
        public async Task<ActionResult> GetRoomDetailByRoomId(string roomId)
        {
            var roomDetail = await _roomService.GetRoomDetailByRoomId(roomId);

            if (roomDetail == null || roomDetail.Rooms.Count == 0)
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Room not found",
                    Data = null
                });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Room details retrieved successfully",
                Data = roomDetail
            });
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetAllRooms(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? roomTypeName,
            [FromQuery] string? district,
            [FromQuery] List<string>? amenityIds)
        {
            var sanitizedAmenityIds = amenityIds ?? new List<string>();

            var rooms = await _roomService.GetAllRoomsAsync(
                minPrice,
                maxPrice,
                roomTypeName,
                district,
                sanitizedAmenityIds
            );

            return Ok(rooms);
        }

        [HttpGet("rooms/{roomId}")]
        public async Task<IActionResult> GetRoomDetail(string roomId)
        {
            var room = await _roomService.GetRoomDetailByIdAsync(roomId);

            if (room == null)
                return NotFound("Room not found");

            return Ok(room);
        }


        [HttpGet("get-past-rooms")]
        public async Task<IActionResult> GetUserPastRooms()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is required" });
                }

                var pastRooms = await _roomService.GetUserPastRooms(token);
                return Ok(new
                {
                    isSuccess = true,
                    message = "Past rooms retrieved successfully",
                    data = pastRooms
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { isSuccess = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { isSuccess = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = "An error occurred while retrieving past rooms", error = ex.Message });
            }
        }
        [HttpGet("landlord/rooms")]
        public async Task<IActionResult> GetRoomsByLandlordToken([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var rooms = await _roomService.GetRoomsByLandlordAsync(token, pageNumber, pageSize);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get room by landlord successfully",
                Data = rooms
            };

            return StatusCode(response.Code, response);
        }

    }
}
