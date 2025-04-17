using System.Linq.Expressions;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.LContractModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;

namespace SP25_RPSC.Services.Service.LandlordContractService
{
    public class LandlordContractService : ILandlordContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IDecodeTokenHandler _decodeTokenHandler;
        public LandlordContractService(IUnitOfWork unitOfWork,
            IMapper mapper, IDecodeTokenHandler decodeTokenHandler) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<List<LandlordContract>> GetCurrentContracts(string LandlordId)
        {
            return await _unitOfWork.LandlordContractRepository.GetContractByLandlordId(LandlordId);
        }
        public async Task<List<LandlordContract>> GetCurrentExpiredContracts(string LandlordId)
        {
            return await _unitOfWork.LandlordContractRepository.GetContractExpiredByLandlordId(LandlordId);
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
        public async Task<ViewLandlordByLandlordIdContractResDTO> GetContractsByLandlordId(string token, int pageIndex, int pageSize, string status, string search)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return new ViewLandlordByLandlordIdContractResDTO
                {
                    Contracts = new List<LandlordContractDetailRes>(),
                    TotalContract = 0
                };
            }

            var landlordId = landlord.LandlordId;
            Expression<Func<LandlordContract, bool>> searchFilter = lc =>
                (string.IsNullOrEmpty(status) || lc.Status == status) && 
                lc.LandlordId == landlordId && 
                (string.IsNullOrEmpty(search) || 
                 lc.Package.Type.Contains(search) || 
                 lc.Package.ServiceDetails.Any(sd => sd.Name.Contains(search))); 

            var landlordContracts = await _unitOfWork.LandlordContractRepository.Get(
                includeProperties: "Landlord,Package,Package.ServiceDetails,Package.ServiceDetails.PricePackages,Landlord.User",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalContracts = await _unitOfWork.LandlordContractRepository.CountAsync(searchFilter);

            if (landlordContracts == null || !landlordContracts.Any())
            {
                return new ViewLandlordByLandlordIdContractResDTO
                {
                    Contracts = new List<LandlordContractDetailRes>(),
                    TotalContract = 0
                };
            }

            var contractResponses = _mapper.Map<List<LandlordContractDetailRes>>(landlordContracts.ToList());

            return new ViewLandlordByLandlordIdContractResDTO
            {
                Contracts = contractResponses,
                TotalContract = totalContracts
            };
        }

        public async Task<LandlordContractDetailRes> GetContractDetailByContractId(string contractId)
        {
            var landlordContract = (await _unitOfWork.LandlordContractRepository.Get(
                filter: lc => lc.LcontractId == contractId,
                includeProperties: "Landlord,Package,Package.ServiceDetails,Landlord.User,ServiceDetail,Package.ServiceDetails.PricePackages"
            )).FirstOrDefault();

            if (landlordContract == null)
            {
                return null;  
            }

            var transactions = await _unitOfWork.TransactionRepository.Get(
                filter: t => t.LcontractId == contractId
            );

            var contractDetailRes = new LandlordContractDetailRes
            {
                LcontractId = landlordContract.LcontractId,
                SignedDate = landlordContract.SignedDate,
                StartDate = landlordContract.StartDate,
                EndDate = landlordContract.EndDate,
                Status = landlordContract.Status,
                CreatedDate = landlordContract.CreatedDate,
                UpdatedDate = landlordContract.UpdatedDate,
                LcontractUrl = landlordContract.LcontractUrl,
                LandlordSignatureUrl = landlordContract.LandlordSignatureUrl,
                PackageName = landlordContract.Package?.Type,
                PackageId = landlordContract.PackageId,
                ServiceDetailId = landlordContract.ServiceDetailId,
                LandlordId = landlordContract.LandlordId,
                ServiceDetailName = landlordContract.ServiceDetail?.Name,
                ServiceDetailDescription = landlordContract.ServiceDetail?.Description,

                Price = landlordContract.Package?.ServiceDetails.FirstOrDefault()?.PricePackages.FirstOrDefault()?.Price ?? 0,
                Duration = int.TryParse(landlordContract.Package.ServiceDetails.FirstOrDefault().Duration, out var duration) ? duration : 0,

                Transactions = transactions.Select(t => new TransactionRes
                {
                    TransactionId = t.TransactionId,
                    TransactionNumber = t.TransactionNumber,
                    TransactionInfo = t.TransactionInfo,
                    Type = t.Type,
                    Amount = t.Amount,
                    PaymentDate = t.PaymentDate,
                    PaymentMethod = t.PaymentMethod,
                    Status = t.Status
                }).ToList()
            };

            return contractDetailRes;
        }





    }
}


