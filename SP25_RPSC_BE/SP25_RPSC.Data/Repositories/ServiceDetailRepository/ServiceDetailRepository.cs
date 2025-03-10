using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ServiceDetailRepository
{
    public interface IServiceDetailRepository : IGenericRepository<ServiceDetail>
    {

    }

    public class ServiceDetailRepository : GenericRepository<ServiceDetail>, IServiceDetailRepository
    {
        private readonly RpscContext _context;

        public ServiceDetailRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
