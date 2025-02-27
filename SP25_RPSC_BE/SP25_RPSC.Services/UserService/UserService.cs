using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.Repositories.UserRepository;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ListCustomerRes>> GetAllCustomer()
        {
            var res = await _unitOfWork.CustomerRepository.GetAllCustomer();
                return res;
        }

        public async Task<IEnumerable<ListLandlordRes>> GetAllLandLord()
        {
           var res = await _unitOfWork.LandlordRepository.GetAllLanlord();
            return res;
        }



    }
}
