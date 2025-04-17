using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.LContractModel.Response;

namespace SP25_RPSC.Services.Service.LandlordContractService
{
    public interface ILandlordContractService
    {
        Task<List<LandlordContract>> GetCurrentContracts(string LandlordId);
        Task<List<LandlordContract>> GetCurrentExpiredContracts(string LandlordId);
        Task DeleteContract(string packageId);
        Task InsertContract(LandlordContract contract);
        Task<ViewLandlordContractResDTO> GetAllLandlordContract(string searchQuery, int pageIndex, int pageSize, string status);
        Task<ViewLandlordByLandlordIdContractResDTO> GetContractsByLandlordId(string token, int pageIndex, int pageSize, string status, string search);
        Task<LandlordContractDetailRes> GetContractDetailByContractId(string contractId);
    }
}
