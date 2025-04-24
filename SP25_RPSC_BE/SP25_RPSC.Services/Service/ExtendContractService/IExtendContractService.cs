using SP25_RPSC.Data.Models.ExtendContract.Request;
using SP25_RPSC.Data.Models.ExtendContract.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.ExtendContractService
{
    public interface IExtendContractService
    {
        Task CreateRequestExtendContractAsync(CreateRequestExtendContract request, string token);
        //    Task<RequestExtendContractResponse> GetRequestExtendContractByIdAsync(int id);
        //    Task<IEnumerable<RequestExtendContractResponseDto>> GetRequestExtendContractsByRentalRoomIdAsync(int rentalRoomId);
        Task RejectExtendContractAsync(string requestId, string messageLandlord, string token);
        Task ApproveExtendContractAsync(string requestId, string messageLandlord, string token);
        Task<PagedViewRequestExtendContractResponse> ViewAllRequestExtendContractAsync(string token, int pageIndex, int pageSize);
        Task<ViewDetailExtendContractResponseModel> ViewDetailRequestExtendContractByContractId(string contractId);
        Task<PagedViewRequestExtendContractResponse> ViewAllRequestExtendContractByCustomerAsync(string token, int pageIndex, int pageSize);
    }
}
