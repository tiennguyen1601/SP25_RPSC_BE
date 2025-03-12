using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SP25_RPSC.Data.Repositories.PaymentRepository
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<Transaction?> GetUnPaidTransactionOfLandlord(string landlordId);
    }

    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly RpscContext _context;

        public TransactionRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetUnPaidTransactionOfLandlord(string landlordId)
        {
            return await _context.Transactions.Where(t => t.Lcontract.LandlordId == landlordId && t.Status.Equals(StatusEnums.Processing))
                                              .Include(c => c.Lcontract)
                                              .FirstOrDefaultAsync();
        }
    }
}
