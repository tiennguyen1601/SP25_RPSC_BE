using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerResponse;

namespace SP25_RPSC.Services.Service.ContractCustomerService
{
    public interface ICustomerContractService
    {
        Task<GetContractsByLandlordResponseModel> GetContractsByLandlordId(string token, string status, string term, int pageIndex, int pageSize);
        Task<CustomerContractResponse> GetContractDetailByContractId(string token, string contractId);
    }
}
