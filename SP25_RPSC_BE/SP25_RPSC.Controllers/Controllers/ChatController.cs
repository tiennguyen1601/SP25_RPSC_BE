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
        private readonly ChatHub _chatHub;

        public ChatController(IChatService chatService,
            ChatHub chatHub)
        {
            _chatService = chatService;
            _chatHub = chatHub;
        }

        [HttpPost("send-message-to-user")]
        public async Task<IActionResult> SendMessageToUser(ChatMessageCreateReqModel chatMessageCreateReqModel)
        {

            var result = await _chatService.AddMessage(chatMessageCreateReqModel);

            string groupName = $"{result.recipientName}_{result.senderName}";

            await _chatHub.Clients.Group(groupName).SendAsync("ReceiveMessage", result.senderName, chatMessageCreateReqModel.message);

            return Ok(new { message = "Message sent." });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] string user1, [FromQuery] string user2)
        {

            var messages = await _chatService.ViewMessageHistory(user1, user2);
            return Ok(messages);
        }
    }
}
