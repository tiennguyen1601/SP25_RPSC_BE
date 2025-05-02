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
    public interface IPostRoomRepository : IGenericRepository<PostRoom>
    {
        Task<List<PostRoom>> GetPostRoomByRoomId(string roomId);
    }

    public class PostRoomRepository : GenericRepository<PostRoom>, IPostRoomRepository
    {
        private readonly RpscContext _context;

        public PostRoomRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PostRoom>> GetPostRoomByRoomId(string roomId)
        {
            return await _context.PostRooms.Where(x => x.RoomId.Equals(roomId)).ToListAsync();
        }
    }
}
