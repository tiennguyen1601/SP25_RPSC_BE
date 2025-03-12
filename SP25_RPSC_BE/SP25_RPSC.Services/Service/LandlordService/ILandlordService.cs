using SP25_RPSC.Data.Entities;
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
    }
}
