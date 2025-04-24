using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomStayModel;

namespace SP25_RPSC.Data.Models.ExtendContract.Response
{
    public class ViewDetailExtendContractResponseModel
    {
        public CustomerContractDto CustomerContract { get; set; } = null!;
        public List<ExtendRequestDto> ExtendRequests { get; set; } = new();
    }

    public class ExtendRequestDto
    {
        public string RequestId { get; set; } = null!;
        public string? Status { get; set; }
        public int? MonthWantToRent { get; set; }
        public string? MessageCustomer { get; set; }
        public string? MessageLandlord { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CustomerName { get; set; }
    }

}
