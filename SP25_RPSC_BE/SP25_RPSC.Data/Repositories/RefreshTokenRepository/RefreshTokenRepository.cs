using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RefreshTokenRepository
{
    public interface IRefreshTokenRepository : IGenericRepository<PricePackage>
    {
        Task<RefreshToken> GetByRefreshToken(string refreshToken);
        Task Add(RefreshToken refreshToken);
        Task Update(RefreshToken refreshToken);
        Task Remove(RefreshToken refreshToken);
    }

    public class RefreshTokenRepository : GenericRepository<PricePackage>, IRefreshTokenRepository
    {
        private readonly RpscContext _context;

        public RefreshTokenRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .Include(rt => rt.User.Role)
                .Where(rt => rt.Token.Equals(refreshToken)).FirstOrDefaultAsync();
        }

        public async Task Add(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
