using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.FeedbackRepository
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {

    }

    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        private readonly RpscContext _context;

        public FeedbackRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
