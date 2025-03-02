using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.Repositories.UserRepository;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetAllUserResponseModel> GetAllCustomer(string searchQuery, int pageIndex, int pageSize)
        {
            Expression<Func<User, bool>> searchFilter = u => string.IsNullOrEmpty(searchQuery) ||
                                                               u.Email.Contains(searchQuery) ||
                                                               u.PhoneNumber.Contains(searchQuery);

            var users = await _unitOfWork.UserRepository.Get(includeProperties: "Customers",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalUser = await _unitOfWork.UserRepository.CountAsync(searchFilter);

            if (users == null || !users.Any())
            {
                return new GetAllUserResponseModel { Users = new List<ListCustomerRes>(), TotalUser = 0 };
            }

            var userResponses = _mapper.Map<List<ListCustomerRes>>(users.ToList());

            return new GetAllUserResponseModel
            {
                Users = userResponses,
                TotalUser = totalUser
            };
        }




        public async Task<IEnumerable<ListLandlordRes>> GetAllLandLord()
        {
            var res = await _unitOfWork.LandlordRepository.GetAllLanlord();
            return res;
        }



    }
}
