using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ChatModel
{
    public class ChatHistoryResModel
    {
        public string ChatId { get; set; } 
        public string LatestMessage { get; set; }  
        public DateTime CreatedAt { get; set; } 

        public ChatUserResModel Receiver { get; set; } 
    }
    public class ChatUserResModel
    {
        public string Id { get; set; }  
        public string Username { get; set; }  
        public string Avatar { get; set; } 
    }


}
