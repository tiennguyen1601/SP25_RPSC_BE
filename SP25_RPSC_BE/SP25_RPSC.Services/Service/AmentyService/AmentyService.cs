using AutoMapper;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.AmentitiesModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.AmentyService
{
    public class AmentyService : IAmentyService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IDecodeTokenHandler _decodeTokenHandler;

        public AmentyService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<bool> CreateAmenty(RoomAmentyRequestCreateModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return false;
            }
            var landlordId = landlord.LandlordId;

            var amenty = new RoomAmenty
            {
                Name = model.Name,
                Compensation = model.Compensation,
                RoomAmentyId = Guid.NewGuid().ToString(),
                LandlordId = landlordId,
            };
            await _unitOfWork.RoomAmentyRepository.Add(amenty);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
