using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomPriceRepository
{
    public interface IRoomPriceRepository : IGenericRepository<RoomPrice>
    {

    }

    public class RoomPriceRepository : GenericRepository<RoomPrice>, IRoomPriceRepository
    {
        private readonly RpscContext _context;

        public RoomPriceRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
