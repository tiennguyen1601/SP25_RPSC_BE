using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.CustomerMoveOutRepository
{
    public interface ICustomerMoveOutRepository : IGenericRepository<CustomerMoveOut>
    {

    }

    public class CustomerMoveOutRepository : GenericRepository<CustomerMoveOut>, ICustomerMoveOutRepository
    {
        private readonly RpscContext _context;

        public CustomerMoveOutRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
