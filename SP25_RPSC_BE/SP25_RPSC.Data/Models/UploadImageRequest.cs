using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models
{
    public class UploadImageRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
