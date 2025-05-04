using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
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
        Task<PagedRoomResult> GetFilteredRoomsAsync(decimal? minPrice, decimal? maxPrice, string roomTypeName, string district, List<string> amenityIds, int pageIndex, int pageSize);
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

        public async Task<PagedRoomResult> GetFilteredRoomsAsync(
    decimal? minPrice,
    decimal? maxPrice,
    string roomTypeName,
    string district,
    List<string> amenityIds,
    int pageIndex = 1,
    int pageSize = 10)
        {
            var baseQuery = _context.Rooms
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
                .Include(r => r.PostRooms)
                .Include(r => r.Feedbacks)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(roomTypeName))
            {
                baseQuery = baseQuery.Where(r => r.RoomType.RoomTypeName == roomTypeName);
            }

            if (!string.IsNullOrEmpty(district))
            {
                baseQuery = baseQuery.Where(r => r.RoomType.Address.District.Contains(district));
            }

            if (minPrice.HasValue)
            {
                baseQuery = baseQuery.Where(r => r.RoomPrices.Any(p => p.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                baseQuery = baseQuery.Where(r => r.RoomPrices.Any(p => p.Price <= maxPrice.Value));
            }

            if (amenityIds != null && amenityIds.Any())
            {
                baseQuery = baseQuery.Where(r => r.RoomAmentiesLists
                    .Any(a => amenityIds.Contains(a.RoomAmenty.Name)));
            }

            var filteredQuery = baseQuery.Where(r => r.PostRooms.Any(pr => pr.Status == "Active"));

            var totalActivePosts = await _context.PostRooms.CountAsync(pr => pr.Status == "Active");

            var totalRooms = await filteredQuery.CountAsync();

            var rooms = await filteredQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedRoomResult
            {
                TotalActivePosts = totalActivePosts,
                TotalRooms = totalRooms,
                Rooms = rooms
            };
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
                .Include(r => r.PostRooms)
                .Where(r =>
                    r.RoomType != null &&
                    r.RoomType.LandlordId == landlordId &&
                    r.PostRooms.Any(pr => pr.Status == "Active")
                )
                .ToListAsync();
        }


    }
}
