using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Transaction?> GetUnpaidTransOfRepresentative(string landlordId)
        {
            return await _unitOfWork.TransactionRepository.GetUnPaidTransactionOfLandlord(landlordId);
        }

        public async Task AddNewTransaction(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.Add(transaction);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateTransaction(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Dictionary<string, decimal>> GetTransactionSummaryByMonth(int? year, DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<Transaction, bool>> filter = t =>
                t.Status == "PAID" &&
                (year == null || t.PaymentDate.HasValue && t.PaymentDate.Value.Year == year) &&
                (startDate == null || t.PaymentDate.HasValue && t.PaymentDate.Value >= startDate) &&
                (endDate == null || t.PaymentDate.HasValue && t.PaymentDate.Value <= endDate);

            var transactions = await _unitOfWork.TransactionRepository.Get(filter: filter);

            if (transactions == null || !transactions.Any())
            {
                return new Dictionary<string, decimal>();
            }

            var monthlyTotal = transactions
                .Where(t => t.PaymentDate.HasValue)
                .GroupBy(t => new { Year = t.PaymentDate.Value.Year, Month = t.PaymentDate.Value.Month })
                .OrderBy(g => g.Key.Year)  
                .ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Year}-{g.Key.Month:D2}",
                    g => (decimal)g.Sum(t => t.Amount)
                );

            return monthlyTotal;
        }



    }
}
