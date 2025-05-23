﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
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
    [FromQuery] List<string>? amenityIds,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10)
        {
            var sanitizedAmenityIds = amenityIds ?? new List<string>();

            var (rooms, totalActivePosts, totalRooms) = await _roomService.GetAllRoomsAsync(
                minPrice,
                maxPrice,
                roomTypeName,
                district,
                sanitizedAmenityIds,
                pageIndex,
                pageSize
            );

            return Ok(new
            {
                TotalActivePosts = totalActivePosts, // Tổng số bài đăng đang hoạt động (PostRoom)
                TotalFilteredRooms = totalRooms,     // Tổng số phòng sau lọc (phục vụ phân trang)
                PageIndex = pageIndex,
                PageSize = pageSize,
                Rooms = rooms
            });
        }

        [HttpGet("feedback/{roomId}")]
        public async Task<IActionResult> GetFeebackDetail(string roomId)
        {
            var room = await _roomService.GetFeedbacksByRoomIdAsync(roomId);

            if (room == null)
                return NotFound("Feedback not found");

            return Ok(room);
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

        [HttpPut]
        [Route("Update-Room/{roomId}")]
        public async Task<ActionResult> UpdateRoom(string roomId, [FromForm] RoomUpdateRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is required" });
                }


                var result = await _roomService.UpdateRoom(roomId, model, token);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Room updated successfully"
                    });
                }

                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Room could not be updated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { isSuccess = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("Inactive-Room/{roomId}")]
        public async Task<ActionResult> Inactive(string roomId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is required" });
                }


                var result = await _roomService.InactiveRoom(roomId, token);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Inactive room successfully"
                    });
                }

                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Room could not be updated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { isSuccess = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("Create-PostRoom")]
        //[Authorize(Roles = "Landlord")]
        public async Task<ActionResult> CreatePostRoom([FromBody] PostRoomCreateRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Unauthorized,
                        Message = "Authorization token is required"
                    });
                }

                var result = await _roomService.CreatePostRoom(token, model);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Post room created successfully"
                    });
                }

                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Post room cannot be created"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("Get-Room-for-post")]
        public async Task<ActionResult> GetRoomForPost()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var rooms = await _roomService.GetAvailableRoomsByLandlordAsync(token);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Room for post successfully",
                Data = rooms
            };

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        [Route("Update-PostRoom/{postRoomId}")]
        public async Task<ActionResult> UpdatePostRoom(string postRoomId, [FromBody] PostRoomUpdateRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Unauthorized,
                        Message = "Authorization token is required"
                    });
                }
                var result = await _roomService.UpdatePostRoom(token, postRoomId, model);
                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Post room updated successfully"
                    });
                }
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Post room cannot be updated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("Inactive-PostRoom/{postRoomId}")]
        public async Task<ActionResult> InactivePostRoom(string postRoomId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Unauthorized,
                        Message = "Authorization token is required"
                    });
                }
                var result = await _roomService.InactivePostRoom(token, postRoomId);
                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Post room deactivated successfully"
                    });
                }
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Post room cannot be deactivated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
    }
}
