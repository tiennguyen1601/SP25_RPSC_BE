using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ExtendCcontractRepository
{
    public interface IExtendCcontractRepository : IGenericRepository<ExtendCcontract>
    {

    }

    public class ExtendCcontractRepository : GenericRepository<ExtendCcontract>, IExtendCcontractRepository
    {
        private readonly RpscContext _context;

        public ExtendCcontractRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
