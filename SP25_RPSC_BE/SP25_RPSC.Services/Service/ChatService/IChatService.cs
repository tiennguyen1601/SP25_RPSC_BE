using SP25_RPSC.Data.Models.ChatModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.ChatService
{
    public interface IChatService
    {
        Task<(bool success, string senderName, string recipientName)> AddMessage(ChatMessageCreateReqModel chatMessageCreateReqModel);
        Task<List<ChatMessageViewResModel>> ViewMessageHistory(string senderId, string receiverId);
    }
}
