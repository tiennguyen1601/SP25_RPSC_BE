using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;

namespace SP25_RPSC.Data.Repositories.RoomRentRequestRepository
{
    public interface IRoomRentRequestRepository : IGenericRepository<RoomRentRequest>
    {
    }
    public class RoomRentRequestRepository : GenericRepository<RoomRentRequest>, IRoomRentRequestRepository
    {
        private readonly RpscContext _context;

        public RoomRentRequestRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

    }
    }