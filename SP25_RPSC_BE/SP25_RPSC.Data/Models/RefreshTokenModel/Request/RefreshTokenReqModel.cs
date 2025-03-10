using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RefreshTokenModel.Request
{
    public class RefreshTokenReqModel
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; }
    }
}
