using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.LContractModel.Response;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordContractService
{
    public class LandlordContractService : ILandlordContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LandlordContractService(IUnitOfWork unitOfWork,
            IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<LandlordContract>> GetCurrentContracts(string LandlordId)
        {
            return await _unitOfWork.LandlordContractRepository.GetContractByLandlordId(LandlordId);
        }

        public async Task DeleteContract(string packageId)
        {
            await _unitOfWork.LandlordContractRepository.Delete(packageId);
            await _unitOfWork.SaveAsync();
        }

        public async Task InsertContract(LandlordContract contract)
        {
            await _unitOfWork.LandlordContractRepository.Add(contract);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ViewLandlordContractResDTO> GetAllLandlordContract(string searchQuery, int pageIndex, int pageSize, string status)
        {
            Expression<Func<LandlordContract, bool>> searchFilter = lc =>
                (string.IsNullOrEmpty(searchQuery) ||
                 lc.Package.Type.Contains(searchQuery) 
                 || lc.Landlord.User.FullName.Contains(searchQuery) || lc.Landlord.User.PhoneNumber.Contains(searchQuery))
                &&
                (string.IsNullOrEmpty(status) || lc.Status == status); 

            var landlordContracts = await _unitOfWork.LandlordContractRepository.Get(
                includeProperties: "Landlord,Package,Package.ServiceDetails,Package.ServiceDetails.PricePackages,Landlord.User",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalContracts = await _unitOfWork.LandlordContractRepository.CountAsync(searchFilter);

            if (landlordContracts == null || !landlordContracts.Any())
            {
                return new ViewLandlordContractResDTO 
                { 
                    Contracts = new List<ListLandlordContractRes>(), TotalContract = 0 
                };
            }

            var contractResponses = _mapper.Map<List<ListLandlordContractRes>>(landlordContracts.ToList());

            return new ViewLandlordContractResDTO
            {
                Contracts = contractResponses,
                TotalContract = totalContracts
            };
        }

    }
}
