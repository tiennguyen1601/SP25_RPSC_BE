using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SP25_RPSC.Data.Models.FeedbackModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.Models.RoomModel.RequestModel;
using SP25_RPSC.Services.Service.FeedbackService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService;
            _logger = logger;
        }

        [HttpGet("landlord/feedbacks")]
        public async Task<IActionResult> GetFeedbacksRoomByLandlord(
            [FromQuery] string? searchQuery,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                var feedbackResponse = await _feedbackService.GetFeedbacksRoomByLandlord(token, searchQuery, pageIndex, pageSize);

                if (feedbackResponse == null || feedbackResponse.Feebacks.Count == 0)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Không có feedbacks cho phòng của landlord này."
                    });
                }

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Danh sách feedbacks được lấy thành công.",
                    Data = feedbackResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackDetailById(string feedbackId)
        {
            try
            {
                var feedbackDetail = await _feedbackService.GetFeedbackDetailById(feedbackId);

                if (feedbackDetail == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Feedback không tồn tại."
                    });
                }

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Lấy chi tiết feedback thành công.",
                    Data = feedbackDetail
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("Create-feedbackRoom")]
        public async Task<ActionResult> CreateFeedBackRoom(FeedBackRoomRequestModel model)

        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _feedbackService.CreateFeedBackRoom(model, token);

            if (result)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "FeedBack created successfully"
                });
            }

            return BadRequest(new { Message = "FeedBack cannot be created" });
        }

        [HttpPost]
        [Route("create-feedback-roommate")]
        public async Task<ActionResult> CreateFeedbackRoommate([FromForm] FeedBackRoommateRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.CreateFeedBackRoommate(model, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Feedback created successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Authorization error: {ex.Message}");
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Resource not found: {ex.Message}");
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid operation: {ex.Message}");
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating feedback: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while processing your request."
                });
            }
        }


        [HttpPut]
        [Route("update-feedback-roommate")]
        public async Task<ActionResult> UpdateFeedbackRoommate([FromForm] UpdateFeedbackRoommateReq request)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.UpdateFeedbackRoommate(request, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Feedback updated successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Authorization error: {ex.Message}");
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Resource not found: {ex.Message}");
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid operation: {ex.Message}");
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating feedback: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while updating the feedback."
                });
            }
        }

        [HttpPut]
        [Route("delete-feedback-roommate/{feedbackId}")]
        public async Task<ActionResult> DeleteFeedbackRoommate(string feedbackId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.DeleteFeedbackRoommate(feedbackId, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Feedback deleted successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Authorization error: {ex.Message}");
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Resource not found: {ex.Message}");
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid operation: {ex.Message}");
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting feedback: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while deleting the feedback."
                });
            }
        }

        [HttpGet]
        [Route("my-feedbacks")]
        public async Task<ActionResult> GetMyFeedbacks()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.GetMyFeedbacks(token);
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = result,
                    Message = "Feedbacks retrieved successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Authorization error: {ex.Message}");
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving feedbacks: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving the feedbacks."
                });
            }
        }

        [HttpPut]
        [Route("update-feedback-room")]
        public async Task<IActionResult> UpdateFeedbackRoom([FromForm] UpdateFeedbackRoomRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.UpdateFeedbackRoom(model, token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Feedback updated successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Authorization error: {ex.Message}");
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Resource not found: {ex.Message}");
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid operation: {ex.Message}");
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating feedback: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while updating the feedback."
                });
            }
        }
        [HttpDelete("delete-feedbackroom/{feedbackId}")]
        public async Task<IActionResult> DeleteFeedbackRoom(string feedbackId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.DeleteFeedbackRoom(feedbackId, token);

                if (result)
                {
                    return StatusCode((int)HttpStatusCode.OK, new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Xóa feedback thành công."
                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Không thể xóa feedback. Vui lòng kiểm tra quyền hạn hoặc thời gian xóa (chỉ được phép trong vòng 3 ngày)."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Data = ex.Message
                });
            }
        }
    }
}
