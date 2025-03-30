
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.UserModels.Request;

namespace SP25_RPSC.Services.Service.CustomerService
{
    public interface ICustomerService
    {
        Task<bool> UpdateInfo(UpdateInfoReq model, string email);
        Task<bool> SendRequestRoomSharing(string token, RoomSharingReq request);
    }
}