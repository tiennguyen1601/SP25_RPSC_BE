using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Services.Service.RoomTypeService;
using System.Net;
using System.Security.Claims;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpGet]
        [Route("Get-All-Pending")]
        public async Task<ActionResult> GetAllRoomTypesPending(int pageIndex, int pageSize)
        {
            var roomTypes = await _roomTypeService.GetAllRoomTypesPending(pageIndex, pageSize);

            if (!roomTypes.Any())
            {
                return NotFound(new { Message = "No room types found with status pending" });
            }

            var response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Fetched room types successfully",
                Data = roomTypes
            };

            return StatusCode(response.Code, response);
        }

        [HttpGet]
        [Route("Get-Detail/{roomTypeId}")]
        public async Task<ActionResult> GetRoomTypeDetail(string roomTypeId)
        {
            var roomTypeDetail = await _roomTypeService.GetRoomTypeDetail(roomTypeId);

            if (roomTypeDetail == null)
            {
                return NotFound(new { Message = "Room type not found" });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Room type details fetched successfully",
                Data = roomTypeDetail
            });
        }

        [HttpPost]
        [Route("Approve/{roomTypeId}")]
        public async Task<ActionResult> ApproveRoomType(string roomTypeId)
        {
            var result = await _roomTypeService.ApproveRoomType(roomTypeId);

            if (result)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Room type approved successfully"
                });
            }

            return BadRequest(new { Message = "Room type cannot be approved, status is not 'Pending'" });
        }

        [HttpPost]
        [Route("Deny/{roomTypeId}")]
        public async Task<ActionResult> DenyRoomType(string roomTypeId)
        {
            var result = await _roomTypeService.DenyRoomType(roomTypeId);

            if (result)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Room type denied successfully"
                });
            }

            return BadRequest(new { Message = "Room type cannot be denied, status is not 'Pending'" });
        }

        [HttpPost]
        [Route("Create-RoomType")]

        public async Task<ActionResult> CreateRoomType(RoomTypeCreateRequestModel model, string token)
        {
            //var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value.ToString();
            var result = await _roomTypeService.CreateRoomType(model, token);

            if (result)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Room type created successfully"
                });
            }

            return BadRequest(new { Message = "Room type cannot be created" });
        }
    }
}
