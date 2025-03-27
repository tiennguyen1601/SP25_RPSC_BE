using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ImageRfRepository
{
    public interface IImageRfRepository : IGenericRepository<ImageRf>
    {
    }

    public class ImageRfRepository : GenericRepository<ImageRf>, IImageRfRepository
    {
        private readonly RpscContext _context;

        public ImageRfRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
