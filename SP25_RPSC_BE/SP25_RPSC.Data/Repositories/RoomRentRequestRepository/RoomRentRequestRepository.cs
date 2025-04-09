using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;

namespace SP25_RPSC.Data.Repositories.RoomRentRequestRepository
{
    public interface IRoomRentRequestRepository : IGenericRepository<RoomRentRequest>
    {
        Task<List<RoomRentRequest>> GetRoomRentRequestByRoomId(string roomId);
    }
    public class RoomRentRequestRepository : GenericRepository<RoomRentRequest>, IRoomRentRequestRepository
    {
        private readonly RpscContext _context;

        public RoomRentRequestRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoomRentRequest>> GetRoomRentRequestByRoomId(string roomId)
        {
            return await _context.RoomRentRequests.Where(x => x.RoomId == roomId)
                                                  .Include(x => x.CustomerRentRoomDetailRequests)
                                                  .ToListAsync();
        }
    }
    }