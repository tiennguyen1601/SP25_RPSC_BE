using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.TransactionService
{
    public interface ITransactionService
    {
        Task<Transaction?> GetUnpaidTransOfRepresentative(string landlordId);
        Task AddNewTransaction(Transaction transaction);
        Task UpdateTransaction(Transaction transaction);
    }
}
