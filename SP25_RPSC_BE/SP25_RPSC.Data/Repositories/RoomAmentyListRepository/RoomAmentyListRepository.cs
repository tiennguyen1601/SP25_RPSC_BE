using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomAmentyListRepository
{
    public interface IRoomAmentyListRepository : IGenericRepository<RoomAmentiesList>
    {

    }

    public class RoomAmentyListRepository : GenericRepository<RoomAmentiesList>, IRoomAmentyListRepository
    {
        private readonly RpscContext _context;

        public RoomAmentyListRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
