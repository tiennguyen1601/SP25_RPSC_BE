using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Transaction?> GetUnpaidTransOfRepresentative(string landlordId)
        {
            return await _unitOfWork.TransactionRepository.GetUnPaidTransactionOfLandlord(landlordId);
        }

        public async Task AddNewTransaction(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.Add(transaction);
        }

        public async Task UpdateTransaction(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.Update(transaction);
        }
    }
}
