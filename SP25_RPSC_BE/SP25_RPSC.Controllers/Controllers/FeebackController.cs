using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
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
            //var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value.ToString();
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
        [Route("Create-feedbackCustomer")]
        public async Task<ActionResult> CreateFeedBackCustomer(FeedBackCustomerRequestModel model)

        {
            //var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value.ToString();
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _feedbackService.CreateFeedBackCustomer(model, token);

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

        [HttpPut("update-feedbackroom/{feedbackId}")]
        public async Task<IActionResult> UpdateFeedbackRoom(string feedbackId, UpdateFeedbackRoomRequestModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _feedbackService.UpdateFeedbackRoom(feedbackId, model, token);

                if (result)
                {
                    return StatusCode((int)HttpStatusCode.OK, new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Cập nhật feedback thành công."
                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Không thể cập nhật feedback. Vui lòng kiểm tra quyền hạn hoặc thời gian chỉnh sửa (chỉ được phép trong vòng 3 ngày)."
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
