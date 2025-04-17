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
        Task<RequestExtendContractResponse> CreateRequestExtendContractAsync(CreateRequestExtendContract request, string token);
        //    Task<RequestExtendContractResponse> GetRequestExtendContractByIdAsync(int id);
        //    Task<IEnumerable<RequestExtendContractResponseDto>> GetRequestExtendContractsByRentalRoomIdAsync(int rentalRoomId);
    }
}
