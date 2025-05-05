using AutoMapper;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.AmentitiesModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
                Status = StatusEnums.Active.ToString(),
                Compensation = model.Compensation,
                RoomAmentyId = Guid.NewGuid().ToString(),
                LandlordId = landlordId,
            };
            await _unitOfWork.RoomAmentyRepository.Add(amenty);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<GetAllAmentiesResponseModel> GetAllAmentiesByLandlordId(string searchQuery, int pageIndex, int pageSize, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            if (landlord == null)
            {
                return new GetAllAmentiesResponseModel { Amenties = new List<ListAmentyRes>(), TotalAmenties = 0 };
            }

            Expression<Func<RoomAmenty, bool>> searchFilter = a =>
                (string.IsNullOrEmpty(searchQuery) || a.Name.Contains(searchQuery))  
                &&
                a.LandlordId == landlord.LandlordId; 

            var amenties = await _unitOfWork.RoomAmentyRepository.Get(
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalAmenties = await _unitOfWork.RoomAmentyRepository.CountAsync(searchFilter);

            if (!amenties.Any())
            {
                return new GetAllAmentiesResponseModel { Amenties = new List<ListAmentyRes>(), TotalAmenties = 0 };
            }

            var amentyResponses = _mapper.Map<List<ListAmentyRes>>(amenties.ToList());

            return new GetAllAmentiesResponseModel
            {
                Amenties = amentyResponses,   
                TotalAmenties = totalAmenties 
            };
        }

        public async Task<bool> UpdateAmenity(RoomAmentyRequestUpdateModel model, string token, string amenityId)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return false;
            }

            var amenity = (await _unitOfWork.RoomAmentyRepository.Get(
                filter: a => a.RoomAmentyId == amenityId && a.LandlordId == landlord.LandlordId
            )).FirstOrDefault();

            var amenityList = (await _unitOfWork.RoomAmentyListRepository.Get(
                filter: x => x.RoomAmentyId == amenity.RoomAmentyId)).FirstOrDefault();

            if (amenityList != null)
            {
                throw new Exception("Cannot update amenity that it already in room");
            }

            if (amenity == null)
            {
                throw new Exception("RoomAmenity not found");
            }

            if (!String.IsNullOrEmpty(model.Name))
            {
                amenity.Name = model.Name;
            }
            if (model.Compensation != null)
            {
                amenity.Compensation = model.Compensation;
            }


            await _unitOfWork.RoomAmentyRepository.Update(amenity);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteAmenity(string token, string amenityId)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return false;
            }

            var amenity = (await _unitOfWork.RoomAmentyRepository.Get(
                filter: a => a.RoomAmentyId == amenityId && a.LandlordId == landlord.LandlordId
            )).FirstOrDefault();

            var amenityList = (await _unitOfWork.RoomAmentyListRepository.Get(
                filter: x => x.RoomAmentyId == amenity.RoomAmentyId)).FirstOrDefault();

            if (amenityList != null)
            {
                throw new Exception("Cannot delete amenity that it already in room");
            }

            if (amenity == null)
            {
                throw new Exception("RoomAmenity not found");
            }

            await _unitOfWork.RoomAmentyRepository.Delete(amenity);
            //await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
