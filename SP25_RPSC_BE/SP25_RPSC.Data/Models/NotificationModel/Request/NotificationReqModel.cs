using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.NotificationModel.Request
{
    public class NotificationReqModel
    {
        public string? NotificationId { get; set; }
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; }
        public string? entity { get; set; }
        public string? entityId { get; set; }
    }
}
