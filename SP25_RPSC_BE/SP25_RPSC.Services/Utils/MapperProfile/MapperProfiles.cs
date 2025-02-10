using AutoMapper;
using SP25_RPSC.Data.Entities;
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
        }
    }
}
