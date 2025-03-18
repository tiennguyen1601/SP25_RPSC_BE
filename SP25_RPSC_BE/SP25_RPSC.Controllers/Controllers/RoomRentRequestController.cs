﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.RoomRentRequestService;
using SP25_RPSC.Services.Service.RoomServices;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomRentRequestController : ControllerBase
    {
        private readonly IRoomRentRequestService _roomRentRequestService;

        public RoomRentRequestController(IRoomRentRequestService roomRentRequestService)
        {
            _roomRentRequestService = roomRentRequestService;
        }

        [HttpGet]
        [Route("Get-Customers-By-RoomRentRequestId")]
        public async Task<ActionResult> GetCustomersByRoomRentRequestsId([FromQuery] string roomRentRequestsId)
        {
            var customers = await _roomRentRequestService.GetCustomersByRoomRentRequestsId(roomRentRequestsId);

            if (customers == null || customers.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No customers found for this request."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get customers successfully.",
                Data = customers
            });
        }

    }
}
