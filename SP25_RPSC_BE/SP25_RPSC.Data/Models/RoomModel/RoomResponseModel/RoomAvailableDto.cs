using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RoomResponseModel
{
    public class RoomAvailableDto
    {
        public string RoomId { get; set; }
        public string? RoomTypeName { get; set; }
        public string? RoomNumber { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public string? FirstImageUrl { get; set; }
    }

}
