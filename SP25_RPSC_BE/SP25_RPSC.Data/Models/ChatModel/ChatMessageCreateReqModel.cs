using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ChatModel
{
    public class ChatMessageCreateReqModel
    {
        [Required]
        public string message { get; set; }
        [Required]
        public string senderId { get; set; }
        [Required]
        public string receiverId { get; set; }
    }

}
