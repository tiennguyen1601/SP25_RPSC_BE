using AutoMapper;
using SP25_RPSC.Data.Models.CustomerRentRoomDetail.CustomerRentRoomDetailResponse;
using SP25_RPSC.Data.UnitOfWorks;

namespace SP25_RPSC.Services.Service.CustomerRentRoomDetailRequestServices
{
    public class CustomerRentRoomDetailRequestService : ICustomerRentRoomDetailRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CustomerRentRoomDetailRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        


    }
}
