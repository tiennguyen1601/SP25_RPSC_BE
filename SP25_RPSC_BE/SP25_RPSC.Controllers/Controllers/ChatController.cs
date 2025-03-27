using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SP25_RPSC.Data.Models.ChatModel;
using SP25_RPSC.Services.Service.ChatService;
using SP25_RPSC.Services.Service.Hubs.ChatHub;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext; 

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpPost("send-message-to-user")]
        public async Task<IActionResult> SendMessageToUser([FromBody] ChatMessageCreateReqModel chatMessageCreateReqModel)
        {
            try
            {
                var (success, senderId, recipientId) = await _chatService.AddMessage(chatMessageCreateReqModel);

                if (!success)
                    return BadRequest(new { message = "Invalid sender or recipient." });

                await _hubContext.Clients.Group(recipientId).SendAsync("ReceiveMessage", senderId, recipientId, chatMessageCreateReqModel.message);

                return Ok(new { message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }



        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] string user2)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var messages = await _chatService.ViewMessageHistory(token, user2);
            return Ok(messages);
        }

        [HttpGet("history-by-user")]
        public async Task<IActionResult> GetHistoryByUserId()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var history = await _chatService.GetHistoryByUserId(token);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
