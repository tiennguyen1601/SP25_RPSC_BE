using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.AmentitiesModel;
using SP25_RPSC.Data.Models.ChatModel;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerResponse;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Data.Models.FeedbackModel.Response;
using SP25_RPSC.Data.Models.LContractModel.Response;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using SP25_RPSC.Data.Models.PostModel.Response;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
using SP25_RPSC.Data.Models.RoomStay;
using SP25_RPSC.Data.Models.RoomStayModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;

namespace SP25_RPSC.Services.Utils.MapperProfile
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Landlord, LandlordResponseUptModel>()
            .ForMember(dest => dest.LandlordId, opt => opt.MapFrom(src => src.LandlordId))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
            .ForMember(dest => dest.NumberRoom, opt => opt.MapFrom(src => src.NumberRoom))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
            .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName))
            .ForMember(dest => dest.BankNumber, opt => opt.MapFrom(src => src.BankNumber))
            .ForMember(dest => dest.Template, opt => opt.MapFrom(src => src.Template))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));

            CreateMap<User, UserResponseModel>()
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
               .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            CreateMap<Customer, CustomerResponseModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
                .ForMember(dest => dest.LifeStyle, opt => opt.MapFrom(src => src.LifeStyle))
                .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.BudgetRange))
                .ForMember(dest => dest.PreferredLocation, opt => opt.MapFrom(src => src.PreferredLocation))
                .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
                .ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType));

            CreateMap<CustomerContract, CustomerContractResponse>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.RentalRoom))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreatedDate))  
                .ForMember(dest => dest.TimeToUpContract, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value.AddDays(-3) : (DateTime?)null))
                .ForMember(dest => dest.DurationOfRental, opt => opt.MapFrom(src =>
                    (src.StartDate.HasValue && src.EndDate.HasValue)
                    ? ((int)((src.EndDate.Value - src.StartDate.Value).TotalDays / 30))
                    : (int?)null));

            CreateMap<RoomAmenty, ListAmentyRes>()
            .ForMember(dest => dest.RoomAmentyId, opt => opt.MapFrom(src => src.RoomAmentyId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Compensation, opt => opt.MapFrom(src => src.Compensation));

            CreateMap<RoomType, GetRoomTypeDetailResponseModel>()
            .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomTypeId))
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomTypeName))
            .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.Deposite))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
            .ForMember(dest => dest.Square, opt => opt.MapFrom(src => src.Square))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.MaxOccupancy, opt => opt.MapFrom(src => src.MaxOccupancy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))  
            .ForMember(dest => dest.RoomServices, opt => opt.MapFrom(src => src.RoomServices)); 

            CreateMap<Address, AddressDto>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.HouseNumber, opt => opt.MapFrom(src => src.HouseNumber))
                .ForMember(dest => dest.Long, opt => opt.MapFrom(src => src.Long))
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.Lat));

            CreateMap<RoomService, RoomServiceRoomTypeDto>()
                        .ForMember(dest => dest.RoomServiceId, opt => opt.MapFrom(src => src.RoomServiceId))
                        .ForMember(dest => dest.RoomServiceName, opt => opt.MapFrom(src => src.RoomServiceName))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<RoomType, ListRoomTypeRes>()
           .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomTypeId))
           .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomTypeName))
           .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.Deposite))
           .ForMember(dest => dest.MaxOccupancy, opt => opt.MapFrom(src => src.MaxOccupancy))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


            CreateMap<Room, RoomResponse>()
                .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomType.RoomTypeId))
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.RoomTypeName))
                .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.RoomType.Deposite))
                .ForMember(dest => dest.MaxOccupancy, opt => opt.MapFrom(src => src.RoomType.MaxOccupancy))
                .ForMember(dest => dest.RoomTypeStatus, opt => opt.MapFrom(src => src.RoomType.Status));

            CreateMap<Customer, UserResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
                .ForMember(dest => dest.LifeStyle, opt => opt.MapFrom(src => src.LifeStyle))
                .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.BudgetRange))
                .ForMember(dest => dest.PreferredLocation, opt => opt.MapFrom(src => src.PreferredLocation))
                .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
                .ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar));



            CreateMap<Customer, ListCustomerRes>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
                .ForMember(dest => dest.LifeStyle, opt => opt.MapFrom(src => src.LifeStyle))
                .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.BudgetRange))
                .ForMember(dest => dest.PreferredLocation, opt => opt.MapFrom(src => src.PreferredLocation))
                .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();

            CreateMap<Landlord, ListLandlordRes>()
              .ForMember(dest => dest.LandlordId, opt => opt.MapFrom(src => src.LandlordId))
              .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
              .ForMember(dest => dest.NumberRoom, opt => opt.MapFrom(src => src.NumberRoom))
              .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();


            CreateMap<Landlord, ListLandlordResgiterResponse>()
                          .ForMember(dest => dest.LandlordId, opt => opt.MapFrom(src => src.LandlordId))
                          .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                          .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                          .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                          .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                          .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.Status))

                          .ReverseMap();
            CreateMap<Landlord, LanlordRegisByIdResponse>()
                         .ForMember(dest => dest.LandlordId, opt => opt.MapFrom(src => src.LandlordId))
                         .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                         .ForMember(dest => dest.NumberRoom, opt => opt.MapFrom(src => src.NumberRoom))
                         .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
                         .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                         .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                         .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                         .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                         .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                         //.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                         .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                         .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                         //.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                         .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.Status))
                         .ForMember(dest => dest.BusinessImageUrls,
                         opt => opt.MapFrom(src => src.BusinessImages.Select(img => img.ImageUrl).ToList()))
                         .ReverseMap();


            CreateMap<ServicePackage, ServicePackageReponse>();

            CreateMap<ServiceDetail, ServiceDetailReponse.ListDetailService>()
                .ForMember(dest => dest.ServiceDetailId, opt => opt.MapFrom(src => src.ServiceDetailId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


            CreateMap<RoomStay, RoomStayDto>()
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Room.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Room.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Room.Location))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Room.RoomImages.Select(img => img.ImageUrl).ToList()));

            CreateMap<RoomStayCustomer, RoomStayCustomerDto>()
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.FullName : null))
               .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.Email : null))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.Address : null))
               .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.Avatar : null))
               .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Customer.Preferences))
                .ForMember(dest => dest.LifeStyle, opt => opt.MapFrom(src => src.Customer.LifeStyle))
                .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.Customer.BudgetRange))
                .ForMember(dest => dest.PreferredLocation, opt => opt.MapFrom(src => src.Customer.PreferredLocation))
                .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Customer.Requirement))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Customer.UserId))
               .ReverseMap();



            CreateMap<RoomStay, RoomStayDetailsDto>()
                .ForMember(dest => dest.RoomStayId, opt => opt.MapFrom(src => src.RoomStayId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room));


            CreateMap<Room, RoomDto>()
                        .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                        .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RoomNumber))
                        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                        .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                        .ForMember(dest => dest.RoomImages, opt => opt.MapFrom(src => src.RoomImages))
                        .ForMember(dest => dest.RoomAmentiesLists, opt => opt.MapFrom(src => src.RoomAmentiesLists))
                        .ForMember(dest => dest.Price, opt => opt.Ignore()) 
                        .ForMember(dest => dest.RoomServices, opt => opt.MapFrom(src => src.RoomType.RoomServices))
                        .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomType.RoomTypeId))
                        .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.RoomTypeName))
                        .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.RoomType.Deposite))
                        .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.RoomType.Area))
                        .ForMember(dest => dest.Square, opt => opt.MapFrom(src => src.RoomType.Square))
                        .ForMember(dest => dest.RoomTypeDescription, opt => opt.MapFrom(src => src.RoomType.Description))
                        .ForMember(dest => dest.MaxOccupancy, opt => opt.MapFrom(src => src.RoomType.MaxOccupancy));


            CreateMap<Room, ListRoomResByRoomTypeId>()
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RoomNumber))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomTypeId))
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.RoomTypeName ?? "N/A"))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.RoomPrices
                .OrderByDescending(p => p.ApplicableDate)
                .FirstOrDefault().Price))
                        .ForMember(dest => dest.RoomImages, opt => opt.MapFrom(src => src.RoomImages.Select(img => img.ImageUrl).ToList()))
                       .ForMember(dest => dest.Amenties, opt => opt.MapFrom(src => src.RoomAmentiesLists));

            CreateMap<RoomAmentiesList, RoomAmentyDto>()
                        .ForMember(dest => dest.RoomAmentyId, opt => opt.MapFrom(src => src.RoomAmentyId))
                        .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomAmenty.Name))  
                        .ForMember(dest => dest.Compensation, opt => opt.MapFrom(src => src.RoomAmenty.Compensation));  


            CreateMap<RoomImage, RoomImageDto>();
            CreateMap<RoomAmentiesList, RoomAmentiesListDto>()
                .ForMember(dest => dest.AmenityId, opt => opt.MapFrom(src => src.RoomAmenty != null ? src.RoomAmenty.RoomAmentyId : null))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomAmenty != null ? src.RoomAmenty.Name : null));

            CreateMap<RoomService, RoomServiceDto>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.RoomServiceId))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.RoomServiceName))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.RoomServicePrices.OrderByDescending(rsp => rsp.ApplicableDate).Select(rsp => rsp.Price).FirstOrDefault()));


            // Định nghĩa ánh xạ cho RoomStay -> RoomStayDetailsResponseModel
            CreateMap<RoomStay, RoomStayDetailsResponseModel>()
                .ForMember(dest => dest.RoomStayId, opt => opt.MapFrom(src => src.RoomStayId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room));

            // Ánh xạ từ RoomStay -> RoomCusDto
            CreateMap<Room, RoomCusDto>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RoomNumber))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.RoomCusImages, opt => opt.MapFrom(src => src.RoomImages)) 
                .ForMember(dest => dest.RoomCusAmentiesLists, opt => opt.MapFrom(src => src.RoomAmentiesLists)) 
                .ForMember(dest => dest.Price, opt => opt.Ignore()) 
                .ForMember(dest => dest.RoomCusServices, opt => opt.MapFrom(src => src.RoomType.RoomServices)) 
                .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomType.RoomTypeId))
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType.RoomTypeName))
                .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.RoomType.Deposite))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.RoomType.Area))
                .ForMember(dest => dest.Square, opt => opt.MapFrom(src => src.RoomType.Square))
                .ForMember(dest => dest.RoomTypeDescription, opt => opt.MapFrom(src => src.RoomType.Description))
                .ForMember(dest => dest.MaxOccupancy, opt => opt.MapFrom(src => src.RoomType.MaxOccupancy));



            // Ánh xạ cho RoomAmentiesList -> RoomAmentiesListDto
            CreateMap<RoomAmentiesList, RoomAmentiesCusListDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomAmenty != null ? src.RoomAmenty.Name : null));

            // Ánh xạ cho RoomService -> RoomServiceDto
            CreateMap<RoomService, RoomServiceCusDto>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.RoomServiceId))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.RoomServiceName))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.RoomServicePrices.OrderByDescending(rsp => rsp.ApplicableDate).Select(rsp => rsp.Price).FirstOrDefault()));

            // Ánh xạ cho RoomImage -> RoomImageDto
            CreateMap<RoomImage, RoomImageCusDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<CustomerContract, CustomerContractDto>()
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.ContractId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Term, opt => opt.MapFrom(src => src.Term))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.RentalRoomId, opt => opt.MapFrom(src => src.RentalRoomId));



            CreateMap<User, UserRegisterReqModel>().ReverseMap();
            CreateMap<User, CustomerRegisterReqModel>().ReverseMap();


            CreateMap<PackageCreateDetailReqestModel, ServiceDetail>()
              .ForMember(dest => dest.PricePackages, opt => opt.MapFrom(src =>
                  new List<PricePackage>
                  {
                    new PricePackage
                    {
                        PriceId =  src.PricePackageModel.PriceId.ToString() ,
                        ApplicableDate = src.PricePackageModel.ApplicableDate
                    }
                  }
              ))
              .ReverseMap();

            CreateMap<Landlord, LandlordRegisterReqModel>().ReverseMap();

            //-------------------------------------ROOMTYPE------------------------------------------
            CreateMap<RoomType, RoomTypeResponseModel>()
            .ForMember(dest => dest.LandlordName, opt => opt.MapFrom(src => src.Landlord.CompanyName))
            .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomTypeId))
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomTypeName))
            .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.Deposite))
            .ForMember(dest => dest.Square, opt => opt.MapFrom(src => src.Square))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<RoomType, RoomTypeDetailResponseModel>()
                .ForMember(dest => dest.LandlordName, opt => opt.MapFrom(src => src.Landlord.CompanyName))
                .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => src.RoomTypeId))
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomTypeName))
                .ForMember(dest => dest.Deposite, opt => opt.MapFrom(src => src.Deposite))
                .ForMember(dest => dest.Square, opt => opt.MapFrom(src => src.Square))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    src.Address != null ? $"{src.Address.HouseNumber}, {src.Address.Street}, {src.Address.District}, {src.Address.City}" : ""))

                //.ForMember(dest => dest.RoomImageUrls, opt => opt.MapFrom(src => src.RoomImages.Select(ri => ri.ImageUrl).ToList()))
                //.ForMember(dest => dest.RoomPrices, opt => opt.MapFrom(src => src.RoomPrices.Select(rp => rp.Price).ToList()))
                .ForMember(dest => dest.RoomServiceNames, opt => opt.MapFrom(src => src.RoomServices.Select(rs => rs.RoomServiceName).ToList()))
                .ForMember(dest => dest.RoomServicePrices, opt => opt.MapFrom(src =>
                    src.RoomServices
                        .SelectMany(rs => rs.RoomServicePrices)
                        .Select(rsp => rsp.Price)
                        .ToList()));


            CreateMap<RoomServiceRequestCreate, RoomService>()
                .ForMember(dest => dest.RoomServicePrices, opt => opt.MapFrom(src =>
                    new List<RoomServicePrice>
                    {
                        new RoomServicePrice
                        {
                            RoomServicePriceId = src.Price.RoomServicePriceId.ToString(),
                            Price = src.Price.Price
                        }
                    }
                )).ReverseMap();

            //----------------------------LandlordContract-------------------------------
            CreateMap<LandlordContract, ListLandlordContractRes>()
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Type))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Package.ServiceDetails.FirstOrDefault().PricePackages.FirstOrDefault().Price))
                .ForMember(dest => dest.LandlordName, opt => opt.MapFrom(src => src.Landlord.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Landlord.User.PhoneNumber))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Package.ServiceDetails.FirstOrDefault().Duration))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.LcontractUrl, opt => opt.MapFrom(src => src.LcontractUrl))
                .ReverseMap();


            CreateMap<LandlordContract, LandlordContractDetailRes>()
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Type)) 
            .ForMember(dest => dest.ServiceDetailName, opt => opt.MapFrom(src => src.Package.ServiceDetails.FirstOrDefault().Name)) 
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Type))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Package.ServiceDetails.FirstOrDefault().PricePackages.FirstOrDefault().Price))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Package.ServiceDetails.FirstOrDefault().Duration))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.LcontractId, opt => opt.MapFrom(src => src.LcontractId))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate)) 
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) 
            .ForMember(dest => dest.LcontractUrl, opt => opt.MapFrom(src => src.LcontractUrl)); 




            //---------------------------Feedback-------------------------------
            CreateMap<Feedback, ListFeedbackRoomeRes>()
           .ForMember(dest => dest.FeedbackID, opt => opt.MapFrom(src => src.FeedbackId))
           .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : "Không xác định"))
           .ForMember(dest => dest.ReviewerPhoneNumber, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.PhoneNumber : "Không có số điện thoại"))
           .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RentalRoom.RoomNumber ?? "Không có số phòng"));

            CreateMap<Feedback, FeedbackDetailResDTO>()
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : "Không xác định"))
            .ForMember(dest => dest.ReviewerPhoneNumber, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.PhoneNumber : "Không có số điện thoại"))
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.RentalRoom.RoomNumber ?? "Không có số phòng"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "Không có nội dung"))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating ?? 0))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.UtcNow))
            .ForMember(dest => dest.ImageUrls, opt => opt.Ignore());


            CreateMap<Chat, ChatMessageViewResModel>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => new Sender
            {
            SenderId = src.Sender.UserId,
            SenderUsername = src.Sender.FullName,
            SenderProfileUrl = src.Sender.Avatar
            }))
            .ForMember(dest => dest.Receiver, opt => opt.MapFrom(src => new Receiver
            {
            ReceiverId = src.Receiver.UserId,
            ReceiverUsername = src.Receiver.FullName,
            ReceiverProfileUrl = src.Receiver.Avatar
            }));

            CreateMap<Address, AddressDTO>();

            CreateMap<RoomType, RoomTypeResModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<Landlord, LandlordResponseModel>()
                .ForMember(dest => dest.LandlordName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<RoomPrice, RoomPriceResponseModel>();

            CreateMap<RoomAmentiesList, RoomAmentyResponseModel>()
                .ForMember(dest => dest.AmenityId, opt => opt.MapFrom(src => src.RoomAmenty.RoomAmentyId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomAmenty.Name));

            CreateMap<Room, RoomResponseModel>()
                .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.RoomType))
                .ForMember(dest => dest.Landlord, opt => opt.MapFrom(src => src.RoomType.Landlord))
                .ForMember(dest => dest.RoomPrices, opt => opt.MapFrom(src => src.RoomPrices))
                .ForMember(dest => dest.RoomImages, opt => opt.MapFrom(src => src.RoomImages.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.RoomAmenities, opt => opt.MapFrom(src => src.RoomAmentiesLists))
                .ForMember(dest => dest.PackageLabel, opt => opt.Ignore())
                .ForMember(dest => dest.PackagePriorityTime, opt => opt.Ignore());

            CreateMap<Room, RoomDetailResponseModel>()
                .IncludeBase<Room, RoomResponseModel>()
                .ForMember(dest => dest.RoomServices, opt => opt.MapFrom(src => src.RoomType.RoomServices));

            CreateMap<RoomType, RoomTypeResModel>();
            CreateMap<Address, AddressDTO>();
            CreateMap<Landlord, LandlordResponseModel>()
                .ForMember(dest => dest.LandlordId, opt => opt.MapFrom(src => src.LandlordId))
                .ForMember(dest => dest.LandlordName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<RoomPrice, RoomPriceResponseModel>();
            CreateMap<RoomService, RoomServiceResponseModel>()
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.RoomServicePrices));
            CreateMap<RoomServicePrice, RoomServicePriceResponseModel>();


            CreateMap<Post, PostViewModel>()
           .ForMember(dest => dest.RoomTitle, opt => opt.MapFrom(src => src.RentalRoom!.Title))
           .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.User!.Email))
           .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User!.FullName))
           .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.User!.PhoneNumber))
           .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.User!.Avatar));
        }
    }
}
