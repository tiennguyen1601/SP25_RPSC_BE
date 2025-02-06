using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.LandlordContractRepository
{
    public interface ILandlordContractRepository : IGenericRepository<LandlordContract>
    {

    }

    public class LandlordContractRepository : GenericRepository<LandlordContract>, ILandlordContractRepository
    {
        private readonly RpscContext _context;

        public LandlordContractRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
