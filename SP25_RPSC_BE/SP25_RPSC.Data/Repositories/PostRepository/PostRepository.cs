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
        Task<IEnumerable<Post>> GetPostsByLandlordUserIdAsync(string landlordId);
        Task<bool> InactivatePostAsync(string postId);
    }

    public class PostRoomRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly RpscContext _context;

        public PostRoomRepository(RpscContext context) : base(context)
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
        public async Task<IEnumerable<Post>> GetPostsByLandlordUserIdAsync(string landlordId)
        {
            return await _context.Posts
                .Include(p => p.RentalRoom)
                    .ThenInclude(r => r.RoomStays)
                        .ThenInclude(rs => rs.Landlord)
                .Include(p => p.User)
                .Where(p => p.Status == "Active" &&
                            p.RentalRoom != null &&
                            p.RentalRoom.RoomStays.Any(rs => rs.Landlord != null &&
                                                             rs.Landlord.UserId == landlordId))
                .ToListAsync();
        }

        public async Task<bool> InactivatePostAsync(string postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post == null) return false;

            post.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
