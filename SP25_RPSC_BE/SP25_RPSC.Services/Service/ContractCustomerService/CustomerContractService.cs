using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerResponse;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;

namespace SP25_RPSC.Services.Service.ContractCustomerService
{
    public class CustomerContractService : ICustomerContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public CustomerContractService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }
        public async Task<GetContractsByLandlordResponseModel> GetContractsByLandlordId(
    string token, string status, string term, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (landlord == null)
            {
                return new GetContractsByLandlordResponseModel { Contracts = new List<CustomerContractResponse>(), TotalContracts = 0 };
            }

            Expression<Func<CustomerContract, bool>> filterExpression = c =>
                c.RentalRoom != null &&
                c.RentalRoom.RoomType != null &&
                c.RentalRoom.RoomType.LandlordId == landlord.LandlordId &&
                (string.IsNullOrEmpty(status) || c.Status == status) &&
                (string.IsNullOrEmpty(term) ||
                 c.ContractId.ToString().Contains(term) ||
                 c.RentalRoom.RoomNumber.Contains(term) ||
                 c.Tenant.User.FullName.Contains(term));

            var contracts = await _unitOfWork.CustomerContractRepository.Get(
                filter: filterExpression,
                includeProperties: "RentalRoom.RoomType,Tenant.User",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalContracts = await _unitOfWork.CustomerContractRepository.CountAsync(filterExpression);

            if (contracts == null || !contracts.Any())
            {
                return new GetContractsByLandlordResponseModel { Contracts = new List<CustomerContractResponse>(), TotalContracts = 0 };
            }

            var contractResponses = _mapper.Map<List<CustomerContractResponse>>(contracts.ToList());

            return new GetContractsByLandlordResponseModel
            {
                Contracts = contractResponses,
                TotalContracts = totalContracts
            };
        }
        public async Task<CustomerContractResponse> GetContractDetailByContractId(string token, string contractId)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found");
            }

            var contract = (await _unitOfWork.CustomerContractRepository.Get(
                filter: c => c.ContractId == contractId && c.RentalRoom.RoomType.LandlordId == landlord.LandlordId,
                includeProperties: "RentalRoom.RoomType,Tenant.User"
            )).FirstOrDefault();

            if (contract == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Contract not found");
            }

            var contractResponse = _mapper.Map<CustomerContractResponse>(contract);

            return contractResponse;
        }




    }
}
