using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomStayCustomerRepository
{
    public interface IRoomStayCustomerRepository : IGenericRepository<RoomStayCustomer>
    {

    }

    public class RoomStayCustomerRepository : GenericRepository<RoomStayCustomer>, IRoomStayCustomerRepository
    {
        private readonly RpscContext _context;

        public RoomStayCustomerRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
