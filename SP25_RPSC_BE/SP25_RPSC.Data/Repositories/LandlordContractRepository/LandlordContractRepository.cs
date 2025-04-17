using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.LandlordContractRepository
{
    public interface ILandlordContractRepository : IGenericRepository<LandlordContract>
    {
        Task RevokeExpirePackages(string LandlordId);
        Task<List<LandlordContract>> GetContractByLandlordId(string LandlordId);
        Task<List<LandlordContract>> GetContractExpiredByLandlordId(string LandlordId);
    }

    public class LandlordContractRepository : GenericRepository<LandlordContract>, ILandlordContractRepository
    {
        private readonly RpscContext _context;

        public LandlordContractRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task RevokeExpirePackages(string LandlordId)
        {
            var uniExpiredPacks = await _context.LandlordContracts
                                    .Where(up => up.LandlordId == LandlordId
                                            && up.EndDate < DateTime.Now
                                            && up.Status.Equals(StatusEnums.Active.ToString())).ToListAsync();

            // revoke 
            foreach (var uni in uniExpiredPacks)
            {
                uni.Status = StatusEnums.Expired.ToString();
                 _context.LandlordContracts.Update(uni);

            }
        }

        public async Task<List<LandlordContract>> GetContractByLandlordId(string LandlordId)
        {
            return await _context.LandlordContracts
                                    .Where(up => up.LandlordId == LandlordId
                                            && up.EndDate > DateTime.Now
                                            && up.Status.Equals(StatusEnums.Active.ToString()))
                                    .Include(up => up.Package)
                                    .ThenInclude(up => up.ServiceDetails)
                                    .Include(t => t.Transactions)
                                    .ToListAsync();
        }
        public async Task<List<LandlordContract>> GetContractExpiredByLandlordId(string LandlordId)
        {
            return await _context.LandlordContracts
                                    .Where(up => up.LandlordId == LandlordId
                                            && up.EndDate < DateTime.Now
                                            && up.Status.Equals(StatusEnums.Expired.ToString()))
                                    .Include(up => up.Package)
                                    .ThenInclude(up => up.ServiceDetails)
                                    .Include(t => t.Transactions)
                                    .ToListAsync();
        }
    }
}
