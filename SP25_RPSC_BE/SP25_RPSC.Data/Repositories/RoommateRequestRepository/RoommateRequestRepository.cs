using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoommateRequestRepository
{
    public interface IRoommateRequestRepository : IGenericRepository<RoommateRequest>
    {
        Task<List<RoommateRequest>> GetRoommateRequestsByPostId(string postId); 
    }

    public class RoommateRequestRepository : GenericRepository<RoommateRequest>, IRoommateRequestRepository
    {
        private readonly RpscContext _context;

        public RoommateRequestRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoommateRequest>> GetRoommateRequestsByPostId(string postId)
        {
            return await _context.RoommateRequests.Where(r => r.PostId.Equals(postId) && 
                                                            r.Status.Equals(StatusEnums.Pending.ToString())).ToListAsync();
        }
    }
}
