using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomServiceRepository
{
    public interface IRoomServiceRepository : IGenericRepository<RoomService>
    {
        Task<List<RoomService>> GetServicesByRoomTypeId(string roomTypeId);
    }

    public class RoomServiceRepository : GenericRepository<RoomService>, IRoomServiceRepository
    {
        private readonly RpscContext _context;

        public RoomServiceRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoomService>> GetServicesByRoomTypeId(string roomTypeId)
        {
            return await _context.RoomServices
                .Include(rs => rs.RoomServicePrices)
                .Where(rs => rs.RoomTypeId == roomTypeId && rs.Status != StatusEnums.Inactive.ToString())
                .ToListAsync();
        }

    }
}
