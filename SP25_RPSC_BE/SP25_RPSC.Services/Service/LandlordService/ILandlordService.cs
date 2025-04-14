using SP25_RPSC.Data.Entities;
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
        Task<bool> AcceptTenantLeaveRoomRequest(string token, string requestId);

    }
}
