using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;

namespace SP25_RPSC.Data.Repositories.BussinessImageRepository
{
    public interface IBussinessImageRepository : IGenericRepository<BusinessImage>
    {

    }


    public class BussinessImageRepository : GenericRepository<BusinessImage>, IBussinessImageRepository
    {
        private readonly RpscContext _context;

        public BussinessImageRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
