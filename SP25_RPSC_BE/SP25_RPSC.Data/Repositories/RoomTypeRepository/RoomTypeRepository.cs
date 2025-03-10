using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomTypeRepository
{
    public interface IRoomTypeRepository : IGenericRepository<RoomType>
    {
        Task<List<RoomType>> GetAllRoomTypesPending(int pageIndex, int pageSize);
        Task<RoomType> GetRoomTypeDetail(string roomTypeId);
        Task<bool> UpdateRoomTypeStatus(string roomTypeId, string status);
    }

    public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
    {
        private readonly RpscContext _context;

        public RoomTypeRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoomType>> GetAllRoomTypesPending(int pageIndex, int pageSize)
        {
            return await _context.RoomTypes
                .Where(rt => rt.Status == "Pending")
                .OrderByDescending(rt => rt.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(rt => rt.Landlord)
                .ToListAsync();
        }

        public async Task<RoomType> GetRoomTypeDetail(string roomTypeId)
        {
            return await _context.RoomTypes
                .Where(rt => rt.RoomTypeId == roomTypeId)
                .Include(rt => rt.Address)
                .Include(rt => rt.RoomImages)
                .Include(rt => rt.RoomPrices)
                .Include(rt => rt.RoomServices) 
                .ThenInclude(rs => rs.RoomServicePrices)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRoomTypeStatus(string roomTypeId, string status)
        {
            var roomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.RoomTypeId == roomTypeId);

            if (roomType == null)
            {
                return false;
            }
            roomType.Status = status;
            _context.RoomTypes.Update(roomType);
            await _context.SaveChangesAsync();
            return true; 
        }
    }
}
