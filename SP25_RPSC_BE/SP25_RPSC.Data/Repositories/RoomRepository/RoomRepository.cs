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
        Task<List<Room>> GetFilteredRoomsAsync(decimal? minPrice, decimal? maxPrice, string roomTypeName, string district, List<string> amenityIds);
        Task<Room> GetRoomByIdAsync(string roomId);
        Task<List<Room>> GetRoomsByLandlordIdAsync(string landlordId);
    }

    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        private readonly RpscContext _context;

        public RoomRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetFilteredRoomsAsync(
            decimal? minPrice,
            decimal? maxPrice,
            string roomTypeName,
            string district,
            List<string> amenityIds)
        {
            var query = _context.Rooms
                .Where(r => r.Status == "Available")
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Address)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.User)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.LandlordContracts)
                            .ThenInclude(c => c.Package)
                .Include(r => r.RoomPrices)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmentiesLists)
                    .ThenInclude(ral => ral.RoomAmenty)
                .AsQueryable();

            if (!string.IsNullOrEmpty(roomTypeName))
            {
                query = query.Where(r => r.RoomType.RoomTypeName == roomTypeName);
            }

            if (!string.IsNullOrEmpty(district))
            {
                query = query.Where(r => r.RoomType.Address.District.Contains(district));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.RoomPrices.Any(p => p.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.RoomPrices.Any(p => p.Price <= maxPrice.Value));
            }

            if (amenityIds != null && amenityIds.Any())
            {
                query = query.Where(r => r.RoomAmentiesLists
                    .Any(a => amenityIds.Contains(a.RoomAmenty.Name)));
            }

            return await query.ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(string roomId)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Address)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.User)
                .Include(r => r.RoomType)
                    .ThenInclude(rt => rt.Landlord)
                        .ThenInclude(l => l.LandlordContracts)
                            .ThenInclude(c => c.Package)
                .Include(r => r.RoomPrices)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmentiesLists)
                    .ThenInclude(ral => ral.RoomAmenty)
                .Include(r => r.RoomType.RoomServices)
                    .ThenInclude(rs => rs.RoomServicePrices)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<List<Room>> GetRoomsByLandlordIdAsync(string landlordId)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .ThenInclude(rt => rt.Address)
                .Include(r => r.RoomImages)
                .Include(r => r.RoomPrices)
                .Include(r => r.RoomAmentiesLists)
                .ThenInclude(ra => ra.RoomAmenty)
                .Include(r => r.RoomType.RoomServices)
                .ThenInclude(rs => rs.RoomServicePrices)
                .Where(r => r.RoomType != null && r.RoomType.LandlordId == landlordId)
                .ToListAsync();
        }
    }
}
