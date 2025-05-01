using SP25_RPSC.Data.Models.PackageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.LContractModel.Request
{
    public class LContractRequestDTO
    {
        public string CompanyName { get; set; } = null!;
        public string LandlordAddress { get; set; } = null!;
        public string LandlordPhone { get; set; } = null!;
        public string LandlordName { get; set; } = null!;
        public DateTime SignedDate { get; set; }
        public DateTime StartDate { get; set; }
        public string PackageName { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public int? PriorityTime { get; set; }
        public int? MaxPost { get; set; }
        public string Label { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public string LandlordSignatureUrl { get; set; } = null!;
    }
}
