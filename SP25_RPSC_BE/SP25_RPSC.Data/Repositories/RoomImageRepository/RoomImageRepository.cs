using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomImageRepository
{
    public interface IRoomImageRepository : IGenericRepository<RoomImage>
    {

    }

    public class RoomImageRepository : GenericRepository<RoomImage>, IRoomImageRepository
    {
        private readonly RpscContext _context;

        public RoomImageRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
