using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.PostRepository
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post> GetById(string PostId);
    }

    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly RpscContext _context;

        public PostRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Post> GetById(string PostId)
        {
            return await _context.Posts.Where(p => p.PostId == PostId)
                .Include(u => u.User).ThenInclude(c => c.Customers)
                .Include(r => r.RentalRoom).ThenInclude(rt => rt.RoomType).ThenInclude(l => l.Landlord).ThenInclude(u => u.User)
                .Include(r => r.RentalRoom).ThenInclude(rt => rt.RoomType).ThenInclude(rs => rs.RoomServices).ThenInclude(rsp => rsp.RoomServicePrices)
                .Include(r => r.RentalRoom).ThenInclude(rt => rt.RoomPrices)
                .Include(r => r.RentalRoom).ThenInclude(ri => ri.RoomImages)
                .Include(r => r.RentalRoom).ThenInclude(al => al.RoomAmentiesLists).ThenInclude(a => a.RoomAmenty)
                .Include(r => r.RentalRoom).ThenInclude(rs => rs.RoomStays).ThenInclude(a => a.RoomStayCustomers)
                .FirstOrDefaultAsync();
        }
    }
}
