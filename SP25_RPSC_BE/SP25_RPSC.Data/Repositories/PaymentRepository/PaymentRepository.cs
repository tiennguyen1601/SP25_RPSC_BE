using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.PaymentRepository
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {

    }

    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly RpscContext _context;

        public PaymentRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
