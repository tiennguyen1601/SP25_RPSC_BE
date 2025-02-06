using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.LandlordRepository
{
    public interface ILandlordRepository : IGenericRepository<Landlord>
    {

    }

    public class LandlordRepository : GenericRepository<Landlord>, ILandlordRepository
    {
        private readonly RpscContext _context;

        public LandlordRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
