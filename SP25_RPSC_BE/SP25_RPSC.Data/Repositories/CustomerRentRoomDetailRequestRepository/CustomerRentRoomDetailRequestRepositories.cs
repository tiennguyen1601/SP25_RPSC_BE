using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;

namespace SP25_RPSC.Data.Repositories.CustomerRentRoomDetailRequestRepository
{
    public interface ICustomerRentRoomDetailRequestRepositories : IGenericRepository<CustomerRentRoomDetailRequest>
    {
        Task<List<CustomerRentRoomDetailRequest>> GetRoomRentRequestByCustomerId(string customerId);
        Task<CustomerRentRoomDetailRequest> GetDetailRequestByRoomRentRequestId(string roomRentRequestsId);
    }

    public class CustomerRentRoomDetailRequestRepositories : GenericRepository<CustomerRentRoomDetailRequest>, ICustomerRentRoomDetailRequestRepositories
    {
        private readonly RpscContext _context;

        public CustomerRentRoomDetailRequestRepositories(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CustomerRentRoomDetailRequest> GetDetailRequestByRoomRentRequestId(string roomRentRequestsId)
        {
            return await _context.CustomerRentRoomDetailRequests
                .Where(x => x.RoomRentRequestsId == roomRentRequestsId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CustomerRentRoomDetailRequest>> GetRoomRentRequestByCustomerId(string customerId)
        {
            return await _context.CustomerRentRoomDetailRequests.Where(x => x.CustomerId == customerId)
                                                                .Include(x => x.RoomRentRequests)
                                                                .ToListAsync();
        }
    }
}

