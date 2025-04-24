using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest
{
    public class CustomerContractRequestDTO
    {
        public string ContractId { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public string PriceInWords { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentDate { get; set; } = null!;

        // Room Info
        public string RoomId { get; set; } = null!;
        public string RoomAddress { get; set; } = null!;
        public string RoomDescription { get; set; } = null!;

        // Landlord Info
        public string LandlordId { get; set; } = null!;
        public string LandlordName { get; set; } = null!;
        public string LandlordAddress { get; set; } = null!;
        public string LandlordPhone { get; set; } = null!;
        public string LandlordSignatureUrl { get; set; } = null!;

        // Customer Info
        public string CustomerId { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string CustomerSignatureUrl { get; set; } = null!;
    }
}
