using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomServicePriceRepository
{
    public interface IRoomServicePriceRepository : IGenericRepository<RoomServicePrice>
    {

    }

    public class RoomServicePriceRepository : GenericRepository<RoomServicePrice>, IRoomServicePriceRepository
    {
        private readonly RpscContext _context;

        public RoomServicePriceRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
