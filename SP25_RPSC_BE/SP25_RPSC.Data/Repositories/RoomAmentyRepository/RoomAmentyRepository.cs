using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomAmentyRepository
{
    public interface IRoomAmentyRepository : IGenericRepository<RoomAmenty>
    {
        Task<bool> DeleteAmenity(RoomAmenty amenity);
    }

    public class RoomAmentyRepository : GenericRepository<RoomAmenty>, IRoomAmentyRepository
    {
        private readonly RpscContext _context;

        public RoomAmentyRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAmenity(RoomAmenty amenity)
        {
            _context.RoomAmenties.Remove(amenity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
