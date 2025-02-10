using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.UserRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task <User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> GetUserByEmail(string email);

    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly RpscContext _context;

        public UserRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNumber));
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
        }
    }
}
