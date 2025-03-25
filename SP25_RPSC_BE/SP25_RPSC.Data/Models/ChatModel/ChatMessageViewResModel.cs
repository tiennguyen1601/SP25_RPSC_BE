using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ChatModel
{
    public class ChatMessageViewResModel
    {
        public string Message { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class Sender
    {
        public String SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderProfileUrl { get; set; }
    }

    public class Receiver
    {
        public String ReceiverId { get; set; }
        public string ReceiverUsername { get; set; }
        public string ReceiverProfileUrl { get; set; }
    }
}
