using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomRepository
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<List<Room>> GetAllRoomsAsync();
    }

    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        private readonly RpscContext _context;

        public RoomRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Where(r => r.Status == "Available")
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.User)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.LandlordContracts)
                            .ThenInclude(c => c.Package)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Address)
                .Include(r => r.RoomPrices)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmentiesLists)
                    .ThenInclude(ral => ral.RoomAmenty)
                .ToListAsync();
        }
    }
}
