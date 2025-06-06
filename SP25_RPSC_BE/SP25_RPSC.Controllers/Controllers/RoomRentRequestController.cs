﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest;
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
            [HttpPost]
            [Route("Accept-Customer-And-Reject-Others")]
            public async Task<ActionResult> AcceptCustomerAndRejectOthers(
                [FromQuery] string roomRentRequestsId,
                [FromQuery] string selectedCustomerId)
            {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _roomRentRequestService.AcceptCustomerAndRejectOthers(
                    token, roomRentRequestsId, selectedCustomerId, HttpContext);

                if (!result)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Failed to process the room rent request."
                    });
                }

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Customer accepted and others rejected successfully."
                });
            }


        [HttpPost]
        [Route("Upload-Contract-Create-RoomStay")]
        public async Task<ActionResult> ConfirmContractAndCreateRoomStay(
[FromForm] ContractUploadRequest request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _roomRentRequestService.ConfirmContractAndCreateRoomStay(
                    token, request);

            if (!result)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to process upload Contract and create roomstay."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Upload Contract and create roomstay successfully."
            });
        }
        [HttpPost]
        [Route("Update-Contract")]
        public async Task<ActionResult> UpdateContract(
[FromForm] ContractUploadRequest request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _roomRentRequestService.UpdateContractTermAsync(
                    token, request);

            if (!result)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to process update Contract ."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update Contract successfully."
            });
        }
    }
}
