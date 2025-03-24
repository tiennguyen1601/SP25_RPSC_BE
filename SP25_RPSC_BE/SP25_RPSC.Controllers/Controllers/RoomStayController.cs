﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.RoomStayService;
using SP25_RPSC.Services.Service.RoomTypeService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomStayController : ControllerBase
    {
        private readonly IRoomStayService _roomStayService;

        public RoomStayController(IRoomStayService roomStayService)
        {
            _roomStayService = roomStayService;
        }
        [HttpGet("by-landlord")]
        public async Task<IActionResult> GetRoomStaysByLandlord(int pageIndex, int pageSize, string query = "")
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _roomStayService.GetRoomStaysByLandlordId(token, query, pageIndex, pageSize);

            if (result == null || result.RoomStays.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No room stays found for this landlord."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get room stays successfully.",
                Data = result
            });
        }

        [HttpGet("customers/{roomStayId}")]
        public async Task<IActionResult> GetRoomStaysCustomerByRoomStayId(string roomStayId)
        {
            var result = await _roomStayService.GetRoomStaysCustomerByRoomStayId(roomStayId);

            if (result == null || result.RoomStayCustomers.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No customers found for this room stay."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get room stay customers successfully.",
                Data = result
            });
        }

    }
}
