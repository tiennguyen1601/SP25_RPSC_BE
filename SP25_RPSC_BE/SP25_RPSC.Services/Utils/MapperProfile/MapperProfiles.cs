using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.UserModels.Request;
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
