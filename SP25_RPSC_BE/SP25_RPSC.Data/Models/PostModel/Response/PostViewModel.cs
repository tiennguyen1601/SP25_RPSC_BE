using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Response
{
    public class PostViewModel
    {
        public string PostId { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? RoomTitle { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAvatar { get; set; }
    }

}
