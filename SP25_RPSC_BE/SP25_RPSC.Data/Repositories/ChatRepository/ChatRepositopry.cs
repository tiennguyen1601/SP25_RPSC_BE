using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ChatRepository
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetMessagesAsync(string senderId, string receiverId);
    }


    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly RpscContext _context;

        public ChatRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Chat>> GetMessagesAsync(string senderId, string receiverId)
        {
            return await _context.Chats
                .Include(x => x.Sender).ThenInclude(x => x.Customers)
                .Include(x => x.Receiver).ThenInclude(x => x.Customers)
                .Include(x => x.Sender).ThenInclude(x => x.Landlords)
                .Include(x => x.Receiver).ThenInclude(x => x.Landlords)
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                       (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.CreateAt)
                .ToListAsync();
        }
    }
}
