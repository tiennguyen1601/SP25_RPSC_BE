using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.ExtendCcontractRepository
{
    public interface IExtendContractRequestRepository : IGenericRepository<ExtendContractRequest>
    {

    }

    public class ExtendContractRequestRepository : GenericRepository<ExtendContractRequest>, IExtendContractRequestRepository
    {
        private readonly RpscContext _context;

        public ExtendContractRequestRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
