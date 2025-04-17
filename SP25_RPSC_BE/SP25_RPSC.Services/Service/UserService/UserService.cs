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
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
using SP25_RPSC.Data.Models.CustomerModel.Request;

namespace SP25_RPSC.Services.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;
        private IDecodeTokenHandler _decodeTokenHandler;


        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryStorageService cloudinaryStorageService
            , IEmailService emailService, IDecodeTokenHandler decodeTokenHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<GetAllUserResponseModel> GetAllCustomer(string searchQuery, int pageIndex, int pageSize, string status)
        {
            Expression<Func<Customer, bool>> searchFilter = c =>
                (string.IsNullOrEmpty(searchQuery) ||
                 c.User.Email.Contains(searchQuery) ||
                 c.User.PhoneNumber.Contains(searchQuery)) 
                &&
                (string.IsNullOrEmpty(status) || c.Status == status)
                &&
                 c.Status != "Pending";

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
             (string.IsNullOrEmpty(status) || c.Status == status)
             &&
             c.Status != "Pending";

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

        public async Task<(int TotalCustomers, int TotalLandlords)> GetTotalUserCounts()
        {
            int totalCustomers = await _unitOfWork.CustomerRepository.CountAsync(c => c.Status == StatusEnums.Active.ToString());
            int totalLandlords = await _unitOfWork.LandlordRepository.CountAsync(l => l.Status == StatusEnums.Active.ToString());

            return (totalCustomers, totalLandlords);
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


            var res = await _unitOfWork.LandlordRepository.Get(orderBy: q => q.OrderByDescending(p => p.CreatedDate),
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



        public async Task<bool> UpdateLandlordStatus(string landlordId, bool isApproved, string rejectionReason = "")
        {
            var landlord = (await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages",
                filter: c => c.LandlordId == landlordId)).FirstOrDefault();

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
                htmlContent = EmailTemplate.LandlordRejection(landlord.User.FullName, rejectionReason);
            }

            landlord.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.LandlordRepository.Update(landlord);
            await _unitOfWork.SaveAsync();

            await _emailService.SendEmail(landlord.User.Email, subject, htmlContent);
            return true;
        }

        public async Task<List<LanlordRegisByIdResponse>> GetProfileLordById(string landlordId)
        {
            var res = await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages",
                filter: c => c.LandlordId.Equals(landlordId)
            );

            var landlordRes = _mapper.Map<List<LanlordRegisByIdResponse>>(res.ToList());
            return landlordRes;
        }


        public async Task<bool> UpdateLandlordProfile(string landlordId, UpdateLandlordProfileRequest model)
        {
            var landlord = (await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages",
                filter: c => c.LandlordId == landlordId)).FirstOrDefault();

            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found.");
            }

            var user = landlord.User;
            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "User not found.");
            }

            if (!string.IsNullOrEmpty(model.CompanyName))
                landlord.CompanyName = model.CompanyName;

            //if (model.NumberRoom.HasValue)
            //    landlord.NumberRoom = model.NumberRoom.Value;

            if (!string.IsNullOrEmpty(model.LicenseNumber))
            {
                var existingLicense = (await _unitOfWork.LandlordRepository.Get(l => l.LicenseNumber == model.LicenseNumber && l.LandlordId != landlordId)).FirstOrDefault();
                if (existingLicense != null)
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "License number already exists!");
                }
                landlord.LicenseNumber = model.LicenseNumber;
            }

            if (!string.IsNullOrEmpty(model.BankName))
                landlord.BankName = model.BankName;

            if (!string.IsNullOrEmpty(model.BankNumber))
            {
                var existingBank = (await _unitOfWork.LandlordRepository.Get(l => l.BankNumber == model.BankNumber && l.LandlordId != landlordId)).FirstOrDefault();
                if (existingBank != null)
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "Bank number already exists!");
                }
                landlord.BankNumber = model.BankNumber;
            }

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserByPhoneNumber(model.PhoneNumber);
                if (existingUser != null && existingUser.UserId != user.UserId)
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "Phone number is already in use!");
                }
                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (model.Dob.HasValue)
                user.Dob = model.Dob;

            if (!string.IsNullOrEmpty(model.Gender))
                user.Gender = model.Gender;

            if (model.Avatar != null)
            {
                var avatarList = new List<IFormFile> { model.Avatar }; 
                var uploadedUrls = await _cloudinaryStorageService.UploadImageAsync(avatarList);
                user.Avatar = uploadedUrls.FirstOrDefault();
            }


            if (landlord.BusinessImages != null && landlord.BusinessImages.Any())
            {
                foreach (var oldImage in landlord.BusinessImages)
                {
                    await _unitOfWork.BussinessImageRepository.Delete(oldImage);
                }
            }

            if (model.BusinessImages != null && model.BusinessImages.Count > 0)
            {
                var downloadUrls = await _cloudinaryStorageService.UploadImageAsync(model.BusinessImages);
                foreach (var link in downloadUrls)
                {
                    var newImage = new BusinessImage
                    {
                        BusinessImageId = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.UtcNow,
                        ImageUrl = link,
                        Status = StatusEnums.Active.ToString(),
                        LandlordId = landlordId
                    };
                    await _unitOfWork.BussinessImageRepository.Add(newImage);
                }
            }

            await _unitOfWork.LandlordRepository.Update(landlord);
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task UpdateUser(User user)
        {
            await _unitOfWork.UserRepository.Update(user);
        }

        public async Task<GetCustomerByUserIdResponseModel> GetCustomerByUserId(string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            
            if (string.IsNullOrEmpty(userId))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "User ID is required.");
            }

            var user = (await _unitOfWork.UserRepository.Get(
                filter: u => u.UserId == userId,
                includeProperties: "Customers"
            )).FirstOrDefault();

            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "User not found.");
            }

            var customer = user.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Customer not found for this user.");
            }

            var userResponse = _mapper.Map<UserResponseModel>(user);
            var customerResponse = _mapper.Map<CustomerResponseModel>(customer);

            return new GetCustomerByUserIdResponseModel
            {
                User = userResponse,
                Customer = customerResponse
            };
        }
        public async Task<bool> UpdateUser(string token, UpdateUserRequestModel model)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var user = (await _unitOfWork.UserRepository.Get(
                filter: u => u.UserId == userId
            )).FirstOrDefault();

            var landlord = (await _unitOfWork.LandlordRepository.Get(
                includeProperties: "User,BusinessImages",
                filter: c => c.UserId == user.UserId)).FirstOrDefault();

            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "User not found.");
            }

            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.Address))
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.Gender))
                user.Gender = model.Gender;

            if (model.Dob.HasValue)
                user.Dob = model.Dob;

            if (model.Avatar != null)
            {
                var avatarList = new List<IFormFile> { model.Avatar };
                var uploadedUrls = await _cloudinaryStorageService.UploadImageAsync(avatarList);
                user.Avatar = uploadedUrls.FirstOrDefault();
            }

            user.UpdateAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return true;
        }
        public async Task<bool> UpdateCustomer(string token, UpdateCustomerRequestModel model)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            
            var customer = (await _unitOfWork.CustomerRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (customer == null)
            {
                
            }
            var customerId = customer.CustomerId;

            if (!string.IsNullOrEmpty(model.Preferences))
                customer.Preferences = model.Preferences;

            if (!string.IsNullOrEmpty(model.LifeStyle))
                customer.LifeStyle = model.LifeStyle;

            if (!string.IsNullOrEmpty(model.BudgetRange))
                customer.BudgetRange = model.BudgetRange;

            if (!string.IsNullOrEmpty(model.PreferredLocation))
                customer.PreferredLocation = model.PreferredLocation;

            if (!string.IsNullOrEmpty(model.Requirement))
                customer.Requirement = model.Requirement;

            if (!string.IsNullOrEmpty(model.CustomerType))
                customer.CustomerType = model.CustomerType;


            await _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return true;
        }
        public async Task<GetLandlordByUserIdResponseModel> GetLandlordByUserId(string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            if (string.IsNullOrEmpty(userId))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "User ID is required.");
            }

            var user = (await _unitOfWork.UserRepository.Get(
                filter: u => u.UserId == userId,
                includeProperties: "Landlords.BusinessImages"  
            )).FirstOrDefault();

            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "User not found.");
            }

            var landlord = user.Landlords.FirstOrDefault(l => l.UserId == userId);

            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found for this user.");
            }

            var userResponse = _mapper.Map<UserResponseModel>(user);
            var landlordResponse = _mapper.Map<LandlordResponseUptModel>(landlord);

            landlordResponse.BusinessImages = landlord.BusinessImages.Select(bi => bi.ImageUrl).ToList();

            return new GetLandlordByUserIdResponseModel
            {
                User = userResponse,
                Landlord = landlordResponse
            };
        }


        public async Task<bool> UpdateLandlord(string token, UpdateLandlordRequestModel model)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (landlord == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Landlord not found.");
            }

            if (!string.IsNullOrEmpty(model.CompanyName))
                landlord.CompanyName = model.CompanyName;

            //if (model.NumberRoom.HasValue)
            //    landlord.NumberRoom = model.NumberRoom;

            if (!string.IsNullOrEmpty(model.LicenseNumber))
                landlord.LicenseNumber = model.LicenseNumber;

            if (!string.IsNullOrEmpty(model.BankName))
                landlord.BankName = model.BankName;

            if (!string.IsNullOrEmpty(model.BankNumber))
                landlord.BankNumber = model.BankNumber;

            if (!string.IsNullOrEmpty(model.Template))
                landlord.Template = model.Template;

            if (!string.IsNullOrEmpty(model.Status))
                landlord.Status = model.Status;

            landlord.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.LandlordRepository.Update(landlord);
            await _unitOfWork.SaveAsync();

            return true;
        }


    }
}
