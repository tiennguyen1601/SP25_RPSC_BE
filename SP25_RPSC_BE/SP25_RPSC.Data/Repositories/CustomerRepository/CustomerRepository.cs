using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        //Task<ListCustomerRes> GetAllCustomer();

        Task<Customer> GetCustomerByUserId(string userId);

    }

    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly RpscContext _context;

        public CustomerRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerByUserId(string userId)
        {
            return await _context.Customers.Where(x => x.User.UserId.Equals(userId)).FirstOrDefaultAsync(); ;
        }

        //public async Task<ListCustomerRes> GetAllCustomer()
        //{
        //    var customers = await _context.Customers
        //                   .Include(c => c.User)
        //                   .Select(c => new ListCustomerRes
        //                   {
        //                       CustomerId = c.CustomerId,
        //                       Preferences = c.Preferences,
        //                       LifeStyle = c.LifeStyle,
        //                       BudgetRange = c.BudgetRange,
        //                       PreferredLocation = c.PreferredLocation,
        //                       Requirement = c.Requirement,
        //                       Status = c.Status,
        //                       Email = c.User.Email,
        //                       FullName = c.User.FullName,
        //                       Address = c.User.Address,
        //                       PhoneNumber = c.User.PhoneNumber,
        //                       Gender = c.User.Gender,
        //                       Avatar = c.User.Avatar,
        //                       UserStatus = c.User.Status
        //                   })
        //                   .ToListAsync();
        //    return customers;
        //}
    }
}
