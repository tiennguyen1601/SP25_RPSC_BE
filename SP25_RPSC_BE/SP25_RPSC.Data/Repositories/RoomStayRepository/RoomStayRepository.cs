using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoomStayRepository
{
    public interface IRoomStayRepository : IGenericRepository<RoomStay>
    {
        //Task<List<Customer>> GetRoommateByCustomerId(string customerId);
    }

    public class RoomStayRepository : GenericRepository<RoomStay>, IRoomStayRepository
    {
        private readonly RpscContext _context;

        public RoomStayRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        
    }
}
