using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest
{
    public class ContractUploadRequest
    {
        [Required]
        public string ContractId { get; set; }

        [Required]
        public IFormFile ContractFile { get; set; }
    }
}
