using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.OTPRepository
{
    public interface IOTPRepository : IGenericRepository<Otp>
    {
        Task<Otp?> GetLatestOTP(string userId);
        Task<Otp?> GetLatestOTPPassword(string userId);
    }

    public class OTPRepository : GenericRepository<Otp>, IOTPRepository
    {
        private readonly RpscContext _context;

        public OTPRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Otp?> GetLatestOTP(string userId)
        {
            var latestOTPList = await _context.Otps.Where(o => o.CreatedBy == userId).FirstOrDefaultAsync();
            return latestOTPList;
        }

        public async Task<Otp?> GetLatestOTPPassword(string userId)
        {
            return await _context.Otps
                .Where(o => o.CreatedBy == userId.ToString() && o.IsUsed == false)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }


    }
}
