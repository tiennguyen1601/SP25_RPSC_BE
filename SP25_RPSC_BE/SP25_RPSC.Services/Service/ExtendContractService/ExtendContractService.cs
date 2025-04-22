using Org.BouncyCastle.Ocsp;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.ExtendContract.Request;
using SP25_RPSC.Data.Models.ExtendContract.Response;
using SP25_RPSC.Data.Models.RoomStayModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.ExtendContractService
{
    public class ExtendContractService : IExtendContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IDecodeTokenHandler _decodeTokenHandler;
        private readonly IEmailService _emailService;

        public ExtendContractService(IUnitOfWork unitOfWork, IDecodeTokenHandler decodeTokenHandler, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _decodeTokenHandler = decodeTokenHandler;
            _emailService = emailService;
        }


        public async Task CreateRequestExtendContractAsync(CreateRequestExtendContract request, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (customer == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Customer not found.");
            }

            var contract = await _unitOfWork.CustomerContractRepository.GetByIDAsync(request.ContractId);
            if (contract == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Contract with ID {request.ContractId} not found.");
            }

            if (contract.TenantId != customer.CustomerId)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Customer is not associated with this contract.");
            }

            if (contract.Status != StatusEnums.Active.ToString())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Only active contracts can be extended.");
            }

            if (request.MonthWantToRent <= 0)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The number of months to extend must be greater than 0.");
            }

            var landlord = await _unitOfWork.LandlordRepository.GetByIDAsync(request.LandlordId);
            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found.");
            }

            var twoMonthsBeforeEndDate = contract.EndDate?.AddMonths(-2);
            if (twoMonthsBeforeEndDate.HasValue && DateTime.UtcNow < twoMonthsBeforeEndDate.Value)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "You can only request an extension within the last 2 months of the contract.");
            }

            var existingRequest = (await _unitOfWork.ExtendContractRequestRepository.Get(
                filter: r => r.ContractId == request.ContractId && r.Status == StatusEnums.Pending.ToString()
            )).FirstOrDefault();

            if (existingRequest != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "There is already a pending request for this contract.");
            }


            var landlordUser = await _unitOfWork.UserRepository.GetByIDAsync(landlord.UserId);
            if (landlordUser != null)
            {
                var email = landlordUser.Email;
                var landlordName = landlordUser.FullName ?? "Landlord";
                var customerUser = await _unitOfWork.UserRepository.GetByIDAsync(customer.UserId);
                var customerName = customerUser?.FullName ?? "Khách thuê";

                var html = EmailTemplate.NotifyLandlordForExtendRequest(
                    landlordName,
                    customerName,
                    contract.ContractId,
                    request.MonthWantToRent,
                    request.MessageCustomer
                );

                await _emailService.SendEmail(email, "Yêu cầu gia hạn hợp đồng mới", html);
            }



            var requestEntity = new ExtendContractRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                MessageCustomer = request.MessageCustomer,
                MonthWantToRent = request.MonthWantToRent,
                Status = StatusEnums.Pending.ToString(),
                LandlordId = request.LandlordId,
                ContractId = request.ContractId,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.ExtendContractRequestRepository.Add(requestEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task RejectExtendContractAsync(string requestId, string messageLandlord, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Not found landlord");
            }
            var landlordId = landlord.LandlordId;

            var request = await _unitOfWork.ExtendContractRequestRepository.GetByIDAsync(requestId);
            if (request == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Extend contract request not found.");
            }

            if (request.LandlordId != landlordId)
            {
                throw new ApiException(HttpStatusCode.Forbidden, "You are not authorized to reject this request.");
            }

            if (request.Status != StatusEnums.Pending.ToString())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Only pending requests can be rejected.");
            }

            request.Status = StatusEnums.Rejected.ToString();
            request.MessageLandlord = messageLandlord;
            _unitOfWork.ExtendContractRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();

            var contract = await _unitOfWork.CustomerContractRepository.GetByIDAsync(request.ContractId);
            if (contract == null)
                return;

            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(contract.TenantId);
            if (customer == null)
                return;

            var user = await _unitOfWork.UserRepository.GetByIDAsync(customer.UserId);
            if (user == null)
                return;

            var email = user.Email;
            var fullname = user.FullName ?? "Customer";

            var html = EmailTemplate.ExtendContractRejected(fullname, contract.ContractId, messageLandlord);
            await _emailService.SendEmail(email, "Yêu cầu gia hạn hợp đồng bị từ chối", html);
        }
        public async Task ApproveExtendContractAsync(string requestId, string messageLandlord, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found.");

            var request = await _unitOfWork.ExtendContractRequestRepository.GetByIDAsync(requestId);
            if (request == null)
                throw new ApiException(HttpStatusCode.NotFound, "Extend contract request not found.");

            if (request.LandlordId != landlord.LandlordId)
                throw new ApiException(HttpStatusCode.Forbidden, "You are not authorized to approve this request.");

            if (request.Status != StatusEnums.Pending.ToString())
                throw new ApiException(HttpStatusCode.BadRequest, "Only pending requests can be approved.");

            request.Status = StatusEnums.Approved.ToString();
            request.MessageLandlord = messageLandlord;
            _unitOfWork.ExtendContractRequestRepository.Update(request);

            var contract = await _unitOfWork.CustomerContractRepository.GetByIDAsync(request.ContractId);
            if (contract == null)
                throw new ApiException(HttpStatusCode.NotFound, "Customer contract not found.");

            var oldEndDate = contract.EndDate ?? DateTime.UtcNow;

            contract.EndDate = oldEndDate.AddMonths(request.MonthWantToRent ?? 1);
            contract.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.CustomerContractRepository.Update(contract);

            if (!string.IsNullOrEmpty(contract.RentalRoomId))
            {
                var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                    filter: r => r.RoomId == contract.RentalRoomId && r.Status == StatusEnums.Active.ToString()
                )).FirstOrDefault();

                if (roomStay != null)
                {
                    roomStay.EndDate = contract.EndDate;
                    roomStay.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.RoomStayRepository.Update(roomStay);
                }
            }

            var previousExtends = await _unitOfWork.ExtendContractRequestRepository.Get(
                filter: e => e.ContractId == contract.ContractId
            );
            int extendCount = previousExtends.Count() + 1;

            var extendHistory = new ExtendCcontract
            {
                ExtendCcontractId = Guid.NewGuid().ToString(),
                StartDateContract = oldEndDate,
                EndDateContract = contract.EndDate ?? DateTime.UtcNow,
                ExtendCount = extendCount,
                ContractId = contract.ContractId
            };

            await _unitOfWork.ExtendCcontractRepository.Add(extendHistory);
            await _unitOfWork.SaveAsync();

            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(contract.TenantId);
            if (customer != null)
            {
                var user = await _unitOfWork.UserRepository.GetByIDAsync(customer.UserId);
                if (user != null)
                {
                    var email = user.Email;
                    var fullname = user.FullName ?? "Customer";
                    var html = EmailTemplate.ExtendContractApproved(
                        fullname,
                        contract.ContractId,
                        contract.EndDate?.ToString("dd/MM/yyyy") ?? "N/A"
                    );
                    await _emailService.SendEmail(email, "Yêu cầu gia hạn hợp đồng đã được chấp nhận", html);
                }
            }
        }
        public async Task<PagedViewRequestExtendContractResponse> ViewAllRequestExtendContractAsync(string token, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return new PagedViewRequestExtendContractResponse();
            }

            var landlordId = landlord.LandlordId;

            Expression<Func<ExtendContractRequest, bool>> filter = r => r.LandlordId == landlordId;

            var total = await _unitOfWork.ExtendContractRequestRepository.CountAsync(filter);

            var requests = await _unitOfWork.ExtendContractRequestRepository.Get(
                filter: filter,
                includeProperties: "Contract,Contract.Tenant,Contract.Tenant.User",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var results = requests.Select(r => new ViewRequestExtendContractResponse
            {
                RequestId = r.RequestId,
                MonthWantToRent = r.MonthWantToRent,
                Status = r.Status,
                MessageCustomer = r.MessageCustomer,
                CreatedAt = r.CreatedAt,
                ContractId = r.ContractId,
                LandlordId = r.LandlordId,
                CustomerName = r.Contract?.Tenant?.User?.FullName ?? "Unknown"
            })
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return new PagedViewRequestExtendContractResponse
            {
                Requests = results,
                TotalRequests = total
            };
        }
        public async Task<ViewDetailExtendContractResponseModel> ViewDetailRequestExtendContractByContractId(string contractId)
        {
            var contract = (await _unitOfWork.CustomerContractRepository.Get(
                filter: c => c.ContractId == contractId,
                includeProperties: "Tenant.User"
            )).FirstOrDefault();

            if (contract == null)
                throw new KeyNotFoundException("Contract not found.");

            var roomStay = (await _unitOfWork.RoomStayRepository.Get(
                filter: rs => rs.RoomId == contract.RentalRoomId && rs.Status == StatusEnums.Active.ToString()
            )).FirstOrDefault();

            var extendRequests = await _unitOfWork.ExtendContractRequestRepository.Get(
                filter: r => r.ContractId == contractId,
                includeProperties: "Landlord.User"
            );

            var contractDto = new CustomerContractDto
            {
                ContractId = contract.ContractId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                Term = contract.Term,
                TenantId = contract.TenantId,
                RentalRoomId = contract.RentalRoomId,
                RoomStayId = roomStay?.RoomStayId
            };

            var customerName = contract.Tenant?.User?.FullName ?? "Unknown";

            var extendDtos = extendRequests.Select(req => new ExtendRequestDto
            {
                RequestId = req.RequestId,
                Status = req.Status,
                MonthWantToRent = req.MonthWantToRent,
                MessageCustomer = req.MessageCustomer,
                MessageLandlord = req.MessageLandlord,
                CreatedAt = req.CreatedAt,
                CustomerName = customerName
            }).ToList();


            return new ViewDetailExtendContractResponseModel
            {
                CustomerContract = contractDto,
                ExtendRequests = extendDtos
            };
        }

        public async Task<PagedViewRequestExtendContractResponse> ViewAllRequestExtendContractByCustomerAsync(string token, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId)).FirstOrDefault();
            if (customer == null)
            {
                return new PagedViewRequestExtendContractResponse();
            }

            var customerId = customer.CustomerId;

            var contracts = await _unitOfWork.CustomerContractRepository.Get(filter: c => c.TenantId == customerId);
            var contractIds = contracts.Select(c => c.ContractId).ToList();

            Expression<Func<ExtendContractRequest, bool>> filter = r => contractIds.Contains(r.ContractId);

            var total = await _unitOfWork.ExtendContractRequestRepository.CountAsync(filter);

            var requests = await _unitOfWork.ExtendContractRequestRepository.Get(
    filter: filter,
    includeProperties: "Contract,Contract.Tenant,Contract.Tenant.User,Contract.RentalRoom,Landlord.User",
    pageIndex: pageIndex,
    pageSize: pageSize
);

            var results = requests.Select(r => new ViewRequestExtendContractResponse
            {
                RequestId = r.RequestId,
                MonthWantToRent = r.MonthWantToRent,
                Status = r.Status,
                MessageCustomer = r.MessageCustomer,
                CreatedAt = r.CreatedAt,
                ContractId = r.ContractId,
                LandlordId = r.LandlordId,
                LandLordName = r.Landlord?.User?.FullName ?? "Unknown",

                RoomId = r.Contract?.RentalRoom?.RoomId,
                RoomTitle = r.Contract?.RentalRoom?.Title,
                RoomNumber = r.Contract?.RentalRoom?.RoomNumber
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToList();


            return new PagedViewRequestExtendContractResponse
            {
                Requests = results,
                TotalRequests = total
            };
        }






    }
}
