using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.AddressRepository
{
    public interface IAddressRepository : IGenericRepository<Address>
    {

    }

    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly RpscContext _context;

        public AddressRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
