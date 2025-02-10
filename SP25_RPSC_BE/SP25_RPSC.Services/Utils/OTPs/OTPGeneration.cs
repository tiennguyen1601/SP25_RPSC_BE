using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Utils.OTPs
{
    public class OTPGeneration
    {
        public static string CreateNewOTPCode()
        {
            return new Random().Next(0, 999999).ToString("D6");
        }
    }
}
