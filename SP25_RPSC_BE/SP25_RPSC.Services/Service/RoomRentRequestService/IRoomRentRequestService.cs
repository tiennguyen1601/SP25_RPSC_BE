using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;

namespace SP25_RPSC.Services.Service.RoomRentRequestService
{
    public interface IRoomRentRequestService
    {
        Task<List<CustomerRequestRes>> GetCustomersByRoomRentRequestsId(string roomRentRequestsId);
        Task<bool> AcceptCustomerAndRejectOthers(string token, string roomRentRequestsId, string selectedCustomerId, HttpContext context);
        Task<bool> ConfirmContractAndCreateRoomStay(string token, ContractUploadRequest request);
    }
}
