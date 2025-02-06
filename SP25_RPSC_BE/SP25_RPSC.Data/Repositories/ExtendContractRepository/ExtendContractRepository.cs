using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ExtendContractRepository
{
    public interface IExtendContractRepository : IGenericRepository<ExtendContract>
    {

    }

    public class ExtendContractRepository : GenericRepository<ExtendContract>, IExtendContractRepository
    {
        private readonly RpscContext _context;

        public ExtendContractRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
