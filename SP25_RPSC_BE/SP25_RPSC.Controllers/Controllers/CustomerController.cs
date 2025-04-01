﻿
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
                    Message = "Send request room sharing successfully.",
                };
                return StatusCode(response.Code, response);
            }
            else {
                ResultModel response = new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to send request room sharing.",
                };
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("get-all-sent-roommate-request")]
        public async Task<IActionResult> GetSentRoommateRequests()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _customerService.GetListSentRequestSharing(token);

                if (result.TotalSentRequests == 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "You have not sent any roommate requests yet.",
                        Data = result
                    });
                }

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get list of sent roommate requests successfully.",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving sent roommate requests: " + ex.Message
                });
            }
        }

        [HttpGet("get-all-roommate-request")]
        public async Task<IActionResult> GetAllRoommateRequest()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _customerService.GetListRequestSharing(token);

                if (result.TotalRequestSharing == 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "You have no roommate requests yet.",
                        Data = result
                    });
                }

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get list roommate request successfully.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                if (ex.Message == "NO_POST")
                {
                    return StatusCode((int)HttpStatusCode.OK, new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "You do not have any post for looking roommate."
                    });
                }

                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
        }

        

        [HttpPut("reject-roommate-request/{requestId}")]
        public async Task<IActionResult> RejectRoommateRequest(string requestId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _customerService.RejectRequestSharing(token, requestId);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Roommate request rejected successfully."
                };

                return StatusCode(response.Code, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while rejecting the roommate request."
                });
            }
        }


        [HttpPut("accept-roommate-request/{requestId}")]
        public async Task<IActionResult> AcceptRoommateRequest(string requestId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _customerService.AcceptRequestSharing(token, requestId);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Roommate request accepted successfully."
                };

                return StatusCode(response.Code, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while accepting the roommate request."
                });
            }
        }
    }
}
