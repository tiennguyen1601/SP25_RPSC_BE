﻿using AutoMapper;
using PdfSharpCore.Pdf.IO;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
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

                .ForMember(dest => dest.RoomImageUrls, opt => opt.MapFrom(src => src.RoomImages.Select(ri => ri.ImageUrl).ToList()))
                .ForMember(dest => dest.RoomPrices, opt => opt.MapFrom(src => src.RoomPrices.Select(rp => rp.Price).ToList()))
                .ForMember(dest => dest.RoomServiceNames, opt => opt.MapFrom(src => src.RoomServices.Select(rs => rs.RoomServiceName).ToList()))
                .ForMember(dest => dest.RoomServicePrices, opt => opt.MapFrom(src =>
                    src.RoomServices
                        .SelectMany(rs => rs.RoomServicePrices)
                        .Select(rsp => rsp.Price)
                        .ToList()));

        }
    }
}
