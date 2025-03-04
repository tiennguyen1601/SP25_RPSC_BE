using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomTypeModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.RoomTypeService
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RoomTypeResponseModel>> GetAllRoomTypesPending(int pageIndex, int pageSize)
        {
            Expression<Func<RoomType, bool>> filter = rt => rt.Status == "Pending";

            var roomTypes = await _unitOfWork.RoomTypeRepository.Get(
                includeProperties: "Landlord",
                filter: filter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: rt => rt.OrderByDescending(r => r.CreatedAt)
            );

            if (roomTypes == null || !roomTypes.Any())
            {
                return new List<RoomTypeResponseModel>();
            }

            var response = _mapper.Map<List<RoomTypeResponseModel>>(roomTypes);

            return response;
        }

        public async Task<RoomTypeDetailResponseModel> GetRoomTypeDetail(string roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetRoomTypeDetail(roomTypeId);

            if (roomType == null)
            {
                return null;
            }

            var response = _mapper.Map<RoomTypeDetailResponseModel>(roomType);

            return response;
        }
    }
}
