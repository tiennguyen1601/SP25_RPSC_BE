using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomRentRequestModel
{
    public class RoomRentRequestCreateModel
    {
        public string RoomId { get; set; }
        public string Message { get; set; }
        public int MonthWantRent { get; set; }
        public DateTime DateWantToRent { get; set; }
    }
}
