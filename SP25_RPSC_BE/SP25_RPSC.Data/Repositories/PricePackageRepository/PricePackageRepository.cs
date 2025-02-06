using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.PricePackageRepository
{
    public interface IPricePackageRepository : IGenericRepository<PricePackage>
    {

    }

    public class PricePackageRepository : GenericRepository<PricePackage>, IPricePackageRepository
    {
        private readonly RpscContext _context;

        public PricePackageRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
