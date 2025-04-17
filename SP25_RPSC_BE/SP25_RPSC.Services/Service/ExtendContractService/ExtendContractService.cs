using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ExtendContract.Request;
using SP25_RPSC.Data.Models.ExtendContract.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.ExtendContractService
{
    public class ExtendContractService : IExtendContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExtendContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestExtendContractResponse> CreateRequestExtendContractAsync(CreateRequestExtendContract request, string t)
        {
            var contract = await _unitOfWork.CustomerContractRepository.GetByIDAsync(request.ContractId);
            if (contract == null)
            {
                throw new ApiException(HttpStatusCode.NotFound,"$Contract with ID {requestDto.ContractId} not found.");
            }

            // Check if this contract belongs to the specified landlord
            var LandlordId = contract.RentalRoom.RoomType.Landlord.LandlordId;
            if (LandlordId == null || LandlordId != request.LandlordId)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The specified landlord is not associated with this contract.");
            }

            // Create new request entity
            var requestEntity = new ExtendContractRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                MessageLandlord = request.MessageLandlord,
                MessageCustomer = request.MessageCustomer,
                MonthWantToRent = request.MonthWantToRent,
                Status = request.Status,
                LandlordId = request.LandlordId,
                ContractId = request.ContractId,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.ExtendContractRequestRepository.Add(requestEntity);
            await _unitOfWork.SaveAsync();

            // Return response DTO
            return new RequestExtendContractResponse
            {
                RequestExtendContractId = requestEntity.RequestId,
                MessageLandlord = requestEntity.MessageLandlord,
                MessageCustomer = requestEntity.MessageCustomer,
                MonthWantToRent = requestEntity.MonthWantToRent,
                Status = requestEntity.Status,
                LandlordId = requestEntity.LandlordId,
                ContractId = requestEntity.ContractId,
                CreatedDate = requestEntity.CreatedAt ?? DateTime.UtcNow
            };
        }

        public Task<RequestExtendContractResponse> GetRequestExtendContractByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
