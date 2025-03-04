using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.LandlordRepository
{
    public interface ILandlordRepository : IGenericRepository<Landlord>
    {
        Task<IEnumerable<ListLandlordRes>> GetAllLanlord();
    }

    public class LandlordRepository : GenericRepository<Landlord>, ILandlordRepository
    {
        private readonly RpscContext _context;

        public LandlordRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ListLandlordRes>> GetAllLanlord()
        {
         var landlords = await _context.Landlords
                        .Include(l => l.User)
                        .Select(l => new ListLandlordRes
                        {
                            LandlordId = l.LandlordId,
                            CompanyName = l.CompanyName,
                            NumberRoom = (int)l.NumberRoom,
                            LiscenseNumber = l.LicenseNumber,
                            Status = l.Status,
                            CreatedDate = (DateTime)l.CreatedDate,
                            UpdatedDate = l.UpdatedDate,
                            Email = l.User.Email,
                            FullName = l.User.FullName,
                            Address = l.User.Address,
                            PhoneNumber = l.User.PhoneNumber,
                            Gender = l.User.Gender,
                            Avatar = l.User.Avatar,
                            UserStatus = l.User.Status
                        })
         .ToListAsync();
            return landlords;
        }
    }
}
