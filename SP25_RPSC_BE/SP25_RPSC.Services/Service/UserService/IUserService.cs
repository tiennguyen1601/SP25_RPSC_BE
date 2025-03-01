using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<ListLandlordRes>> GetAllLandLord();
        Task<IEnumerable<ListCustomerRes>> GetAllCustomer();
    }
}
