using SP25_RPSC.Data.Entities;
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

    }

    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly RpscContext _context;

        public TransactionRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
