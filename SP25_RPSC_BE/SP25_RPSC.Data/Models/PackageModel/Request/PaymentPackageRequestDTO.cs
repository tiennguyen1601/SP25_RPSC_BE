using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PackageModel.Request
{
    public class PaymentPackageRequestDTO
    {
        public string LandlordId { get; set; } = null!;
        public string PackageId { get; set; } = null!;
        public string ServiceDetailId { get; set; } = null!;
        public IFormFile SignatureFile { get; set; } = null!;
    }
}
