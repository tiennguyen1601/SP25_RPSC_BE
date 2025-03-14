using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.UserService
{
    public interface IUserService
    {
        Task<GetAllLandlordResponseModel> GetAllLandLord(string searchQuery, int pageIndex, int pageSize, string status);
        Task<GetAllUserResponseModel> GetAllCustomer(string searchQuery, int pageIndex, int pageSize, string status);

        Task RegisterLandlord(LandlordRegisterReqModel model, string email);
        Task<GetAllLandlordRegisterResponseModel> GetRegisLandLord(string searchQuery, int pageIndex, int pageSize);
        Task<bool> UpdateLandlordStatus(string landlordId, bool isApproved);
        Task<List<LanlordRegisByIdResponse>> GetRegisLandLordById(string landlordId);
        Task<List<LanlordRegisByIdResponse>> GetProfileLordById(string landlordId);
        Task<bool> UpdateLandlordProfile(string landlordId, UpdateLandlordProfileRequest model);
    }
}
