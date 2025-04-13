using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.NotificationModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.NotificationService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationReqModel notificationReqModel)
        {
            await _notificationService.SendNotificationToUser(notificationReqModel);
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Send notification successfully",
            };
            return StatusCode(response.Code, response);
        }
    }
}
