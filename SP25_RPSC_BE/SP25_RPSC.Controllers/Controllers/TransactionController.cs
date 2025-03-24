using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.TransactionService;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("Get-Transaction-Summary")]
        public async Task<ActionResult> GetTransactionSummary(int? year, DateTime? startDate, DateTime? endDate)
        {
            var transactionSummary = await _transactionService.GetTransactionSummaryByMonth(year, startDate, endDate);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get transaction summary successfully",
                Data = transactionSummary
            };

            return StatusCode(response.Code, response);
        }
    }
}
