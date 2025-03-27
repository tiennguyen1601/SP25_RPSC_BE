using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ServicePackageRepository
{
    public interface IServicePackageRepository : IGenericRepository<ServicePackage>
    {
        Task<ServicePackage> GetServicePackageById(string id);
        Task<ServicePackage?> GetPackageById(string packageId);
    }

    public class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
    {
        private readonly RpscContext _context;

        public ServicePackageRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ServicePackage> GetServicePackageById(string id)
        {
            return await _context.ServicePackages.Include(x=> x.ServiceDetails)
                                                 .ThenInclude(x=> x.PricePackages)
                                                 .FirstOrDefaultAsync(x => x.PackageId.Equals(id));
        }

        public async Task<ServicePackage?> GetPackageById(string packageId)
        {
            return await _context.ServicePackages.Where(u => u.PackageId == packageId)
                .Include(u => u.ServiceDetails).ThenInclude(u => u.PricePackages)
                .FirstOrDefaultAsync();
        }
    }
}
