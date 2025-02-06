using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.CustomerContractRepository
{
    public interface ICustomerContractRepository : IGenericRepository<CustomerContract>
    {

    }

    public class CustomerContractRepository : GenericRepository<CustomerContract>, ICustomerContractRepository
    {
        private readonly RpscContext _context;

        public CustomerContractRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
