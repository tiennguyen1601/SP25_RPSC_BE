using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomRentModel
{
    public class RoomRentResponseModel
    {
        public string RoomRequestId { get; set; }
        public string RoomId { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Message { get; set; }
        public int? MonthWantRent { get; set; }
        public DateTime? DateWantToRent { get; set; }
    }
}
