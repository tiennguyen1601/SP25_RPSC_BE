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

    }

    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly RpscContext _context;

        public PostRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
