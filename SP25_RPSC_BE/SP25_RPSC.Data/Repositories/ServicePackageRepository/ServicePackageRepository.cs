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

    }

    public class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
    {
        private readonly RpscContext _context;

        public ServicePackageRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
