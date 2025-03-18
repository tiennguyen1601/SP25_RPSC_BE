using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;

namespace SP25_RPSC.Data.Repositories.CustomerRentRoomDetailRequestRepository
{
    public interface ICustomerRentRoomDetailRequestRepositories : IGenericRepository<CustomerRentRoomDetailRequest>
    {
    }

    public class CustomerRentRoomDetailRequestRepositories : GenericRepository<CustomerRentRoomDetailRequest>, ICustomerRentRoomDetailRequestRepositories
    {
        private readonly RpscContext _context;

        public CustomerRentRoomDetailRequestRepositories(RpscContext context) : base(context)
        {
            _context = context;
        }

    }
}

