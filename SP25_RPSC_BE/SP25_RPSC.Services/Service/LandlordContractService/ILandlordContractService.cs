using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordContractService
{
    public interface ILandlordContractService
    {
        Task<List<LandlordContract>> GetCurrentContracts(string LandlordId);
        void DeleteContract(string packageId);
        Task InsertContract(LandlordContract contract);
    }
}
