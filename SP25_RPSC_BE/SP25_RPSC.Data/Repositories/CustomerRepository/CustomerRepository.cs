using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {

    }

    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly RpscContext _context;

        public CustomerRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
