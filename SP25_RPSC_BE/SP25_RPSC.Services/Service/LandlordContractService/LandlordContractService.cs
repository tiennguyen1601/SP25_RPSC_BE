using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordContractService
{
    public class LandlordContractService : ILandlordContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LandlordContractService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LandlordContract>> GetCurrentContracts(string LandlordId)
        {
            return await _unitOfWork.LandlordContractRepository.GetContractByLandlordId(LandlordId);
        }

        public void DeleteContract(string packageId)
        {
            _unitOfWork.LandlordContractRepository.Delete(packageId);
        }

        public async Task InsertContract(LandlordContract contract)
        {
            await _unitOfWork.LandlordContractRepository.Add(contract);
        }



    }
}
