using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.Email;
using SP25_RPSC.Services.Service.EmailService;

namespace SP25_RPSC.Services.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
        }

        public async Task<GetAllUserResponseModel> GetAllCustomer(string searchQuery, int pageIndex, int pageSize, string status)
        {
            Expression<Func<Customer, bool>> searchFilter = c =>
                (string.IsNullOrEmpty(searchQuery) ||
                 c.User.Email.Contains(searchQuery) ||
                 c.User.PhoneNumber.Contains(searchQuery)) 
                &&
                (string.IsNullOrEmpty(status) || c.Status == status);

            var customers = await _unitOfWork.CustomerRepository.Get(
                includeProperties: "User",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalUser = await _unitOfWork.CustomerRepository.CountAsync(searchFilter);

            if (customers == null || !customers.Any())
            {
                return new GetAllUserResponseModel { Users = new List<ListCustomerRes>(), TotalUser = 0 };
            }

            var userResponses = _mapper.Map<List<ListCustomerRes>>(customers.ToList());

            return new GetAllUserResponseModel
            {
                Users = userResponses,
                TotalUser = totalUser
            };
        }

        public async Task<GetAllLandlordResponseModel> GetAllLandLord(string searchQuery, int pageIndex, int pageSize, string status)
        {
            Expression<Func<Landlord, bool>> searchFilter = c =>
             (string.IsNullOrEmpty(searchQuery) ||
              c.User.Email.Contains(searchQuery) ||
              c.User.PhoneNumber.Contains(searchQuery))
             &&
             (string.IsNullOrEmpty(status) || c.Status == status);

            var res = await _unitOfWork.LandlordRepository.Get(includeProperties: "User",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var totalLandlord = await _unitOfWork.LandlordRepository.CountAsync(searchFilter);

            if (res == null || !res.Any())
            {
                return new GetAllLandlordResponseModel { Landlords = new List<ListLandlordRes>(), TotalUser = 0 };
            }

            var landlordRes = _mapper.Map<List<ListLandlordRes>>(res.ToList());

            return new GetAllLandlordResponseModel
            {
                Landlords = landlordRes,
                TotalUser = totalLandlord
            };
        }

        public async Task RegisterLandlord(LandlordRegisterReqModel model, string email)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmail(email);
            if (existingUser == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Not Found User");
            }

            var existingLicense = (await _unitOfWork.LandlordRepository
                .Get(l => l.LicenseNumber == model.LicenseNumber)).FirstOrDefault();
            if (existingLicense != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "License number already exists!");
            }

            var existingBank = (await _unitOfWork.LandlordRepository
                .Get(l => l.BankNumber == model.BankNumber)).FirstOrDefault();
            if (existingBank != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Bank number already exists!");
            }

            var newLanlord = _mapper.Map<Landlord>(model);
            newLanlord.LandlordId = Guid.NewGuid().ToString();
            newLanlord.Status = StatusEnums.Pending.ToString();
            newLanlord.CreatedDate = DateTime.Now;
            newLanlord.UpdatedDate = DateTime.Now;
            newLanlord.User = existingUser;

            var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(model.WorkshopImages);
            foreach (var link in downloadUrl)
            {
                var Image = new BusinessImage
                {
                    BusinessImageId = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now,
                    ImageUrl = link,
                    Status = StatusEnums.Active.ToString(),
                };

                newLanlord.BusinessImages.Add(Image);
            }

            await _unitOfWork.LandlordRepository.Add(newLanlord);
            await _unitOfWork.SaveAsync();
        }


        public async Task<GetAllLandlordRegisterResponseModel> GetRegisLandLord(string searchQuery, int pageIndex, int pageSize)
        {
            Expression<Func<Landlord, bool>> searchFilter = c =>
                c.Status == StatusEnums.Pending.ToString() &&
                (string.IsNullOrEmpty(searchQuery) ||
                 c.User.Email.Contains(searchQuery) ||
                 c.User.PhoneNumber.Contains(searchQuery));


            var res = await _unitOfWork.LandlordRepository.Get(orderBy: q => q.OrderBy(p => p.CreatedDate),
                                                                    includeProperties: "User,BusinessImages", 
                                                                    filter: searchFilter,
                                                                    pageIndex: pageIndex,
                                                                    pageSize: pageSize);


            var totalLandlord = await _unitOfWork.LandlordRepository.CountAsync(searchFilter);

            if (res == null || !res.Any())
            {
                return new GetAllLandlordRegisterResponseModel { Landlords = new List<ListLandlordResgiterResponse>(), TotalUser = 0 };
            }

            var landlordRes = _mapper.Map<List<ListLandlordResgiterResponse>>(res.ToList());

            return new GetAllLandlordRegisterResponseModel
            {
                Landlords = landlordRes,
                TotalUser = totalLandlord
            };
        }

        public async Task<List<LanlordRegisByIdResponse>> GetRegisLandLordById(string landlordId)
        {
            var res = await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages",
                filter: c => c.LandlordId.Equals(landlordId)
            );

            var landlordRes = _mapper.Map<List<LanlordRegisByIdResponse>>(res.ToList());
            return landlordRes;
        }



        public async Task<bool> UpdateLandlordStatus(string landlordId, bool isApproved)
        {
            var landlord = (await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages"
                , filter:c => c.LandlordId == landlordId)).FirstOrDefault();

            if (landlord == null || landlord.Status == null)
            {
                return false;
            }

            string subject, htmlContent;
            if (isApproved)
            {
                landlord.Status = StatusEnums.Active.ToString();
                subject = "Chúc mừng! Bạn đã được phê duyệt";
                htmlContent = EmailTemplate.LandlordApproval(landlord.User.FullName);
            }
            else
            {
                landlord.Status = StatusEnums.Deactive.ToString();
                subject = "Thông báo từ chối yêu cầu đăng ký";
                htmlContent = EmailTemplate.LandlordRejection(landlord.User.FullName);
            }

            landlord.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.LandlordRepository.Update(landlord);
            await _unitOfWork.SaveAsync();


            await _emailService.SendEmail(landlord.User.Email, subject, htmlContent);
            return true;
        }




    }
}
