using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailRequest
{
    public class AcceptCustomerRequest
    {
        public string RoomRentRequestsId { get; set; }
        public string SelectedCustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
