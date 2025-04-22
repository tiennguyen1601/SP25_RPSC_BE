using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ExtendContract.Response
{

    public class PagedViewRequestExtendContractResponse
    {
        public List<ViewRequestExtendContractResponse> Requests { get; set; } = new();
        public int TotalRequests { get; set; }
    }

    public class ViewRequestExtendContractResponse
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
        public int? MonthWantToRent { get; set; }
        public string? MessageCustomer { get; set; }
        public string? MessageLandlord { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? ContractId { get; set; }
        public string? LandlordId { get; set; }
        public string? CustomerName { get; set; }
        public string? LandLordName { get; set; }

        public string? RoomId { get; set; }
        public string? RoomTitle { get; set; }
        public string? RoomNumber { get; set; }

    }

}
