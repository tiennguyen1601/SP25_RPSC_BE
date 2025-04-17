using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.LContractModel.Response
{
    public class ViewLandlordByLandlordIdContractResDTO
    {
        public int TotalContract { get; set; }
        public List<LandlordContractDetailRes> Contracts { get; set; }

    }
    public class LandlordContractDetailRes
    {
        public string LcontractId { get; set; }
        public string? LandlordId { get; set; }

        public DateTime? SignedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? LcontractUrl { get; set; }
        public string? LandlordSignatureUrl { get; set; }
        public string? PackageName { get; set; }
        public string? PackageId { get; set; }

        public string? ServiceDetailName { get; set; }
        public string? ServiceDetailId { get; set; }

        public string? ServiceDetailDescription { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
        public List<TransactionRes> Transactions { get; set; } = new List<TransactionRes>();
    }
    public class TransactionRes
    {
        public string TransactionId { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionInfo { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; }
    }


}
