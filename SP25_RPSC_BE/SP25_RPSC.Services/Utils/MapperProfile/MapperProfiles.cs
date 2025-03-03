using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Utils.MapperProfile
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles() 
        {
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

            CreateMap<ServicePackage, ServicePackageReponse>();

            CreateMap<ServiceDetail, ServiceDetailReponse>()
                .ForMember(dest => dest.ServiceDetailId, opt => opt.MapFrom(src => src.ServiceDetailId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.LimitPost, opt => opt.MapFrom(src => src.LimitPost))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId));



            CreateMap<User, UserRegisterReqModel>().ReverseMap();
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
                }
    }
}
