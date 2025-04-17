
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Data.Models.UserModels.Request;

namespace SP25_RPSC.Services.Service.CustomerService
{
    public interface ICustomerService
    {
        Task<bool> UpdateInfo(UpdateInfoReq model, string email);
        Task<bool> SendRequestRoomSharing(string token, RoomSharingReq request);
        Task<bool> CancelRequestRoomSharing(string token, string requestId);
        Task<ListRequestSharingRes> GetListRequestSharing(string token);
        Task<ListSentRequestSharingRes> GetListSentRequestSharing(string token);
        Task<bool> RejectRequestSharing(string token, RejectSharingReq rejectSharingReq);
        Task<bool> AcceptRequestSharing(string token, string requestId);
        Task<bool> RequestLeaveRoom(string token);
        Task<ListLeaveRoomRes> GetListLeaveRoomRequest(string token);
        Task<bool> AcceptLeaveRoomRequest(string token, string requestId);
        Task<bool> RequestLeaveRoomByTenant(string token, TenantRoomLeavingReq request);
        Task<bool> KickRoommateByTenant(string token, KickRoommateReq kickRoommateReq);


    }
}