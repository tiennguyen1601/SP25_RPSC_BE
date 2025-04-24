using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.LContractModel.Response
{
    public class ViewLandlordContractResDTO
    {
        public int TotalContract { get; set; }
        public List<ListLandlordContractRes> Contracts { get; set; }

    }

    public class ListLandlordContractRes
    {
        public string PackageName { get; set; }

        public decimal? Price { get; set; }

        public string? LandlordName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Duration { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Status { get; set; }

        public string? LcontractUrl { get; set; }
    }
}
