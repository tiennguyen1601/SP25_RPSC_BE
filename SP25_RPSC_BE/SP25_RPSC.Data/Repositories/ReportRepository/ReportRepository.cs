using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ReportRepository
{
    public interface IReportRepository : IGenericRepository<Report>
    {

    }

    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly RpscContext _context;

        public ReportRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
