using AutoMapper;
using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.CustomerService
{


    public class CustomerService : ICustomerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
        }

        public async Task<bool> UpdateInfo(UpdateInfoReq model, string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email)
                ?? throw new ApiException(HttpStatusCode.NotFound, "Not Found User");

            var customer = await _unitOfWork.CustomerRepository.GetCustomerByUserId(user.UserId)
                ?? throw new ApiException(HttpStatusCode.BadRequest, "Customer not exists or not active!");

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserByPhoneNumber(model.PhoneNumber);
                if (existingUser != null && existingUser.UserId != user.UserId)
                    throw new ApiException(HttpStatusCode.BadRequest, "Phone number is already in use!");

                user.PhoneNumber = model.PhoneNumber;
            }

            if (model.Avatar != null)
            {
                var avatarList = new List<IFormFile> { model.Avatar };
                var uploadedUrls = await _cloudinaryStorageService.UploadImageAsync(avatarList);
                user.Avatar = uploadedUrls.FirstOrDefault();
            }

            user.Dob = model.Dob ?? user.Dob;
            user.Gender = model.Gender ?? user.Gender;

            customer.Preferences = model.Preferences ?? customer.Preferences;
            customer.LifeStyle = model.LifeStyle ?? customer.LifeStyle;
            customer.BudgetRange = model.BudgetRange ?? customer.BudgetRange;
            customer.PreferredLocation = model.PreferredLocation ?? customer.PreferredLocation;
            customer.Requirement = model.Requirement ?? customer.Requirement;
            customer.CustomerType = model.CustomerType?.ToString() ?? customer.CustomerType;
            customer.Status = StatusEnums.Active.ToString();

            user.UpdateAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return true;
        }


    }
}
