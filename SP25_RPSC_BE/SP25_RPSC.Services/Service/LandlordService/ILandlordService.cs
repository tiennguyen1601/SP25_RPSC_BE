using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordService
{
    public interface ILandlordService
    {
        Task<Landlord?> GetLandlordById(string id);
        Task<ListTenantLeaveRoomRes> GetListTenantLeaveRoomRequest(string token);
        Task<DetailTenantLeaveRoomRes> GetDetailTenantLeaveRoomRequest(string cmoId);
        Task<bool> AcceptTenantLeaveRoomRequest(string token, string requestId);
        Task<bool> KickTenantbyLanlord(string token, KickTenantReq request);
    }
}
