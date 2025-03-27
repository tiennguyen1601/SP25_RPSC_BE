using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.ContractCustomerService;
using SP25_RPSC.Services.Service.RoomServices;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractCustomerController : ControllerBase
    {
        private readonly ICustomerContractService _customerContractService;

        public ContractCustomerController(ICustomerContractService customerContractService)
        {
            _customerContractService = customerContractService;
        }

        [HttpGet("Get-Customer-Contracts")]
        public async Task<ActionResult> GetCustomerContracts(
                            [FromQuery] string? status,
                            [FromQuery] string? term,
                            [FromQuery] int? pageIndex,
                            [FromQuery] int? pageSize)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var contracts = await _customerContractService.GetContractsByLandlordId(
                token, status, term, pageIndex ?? 1, pageSize ?? 10
            );

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get customer contracts successfully",
                Data = contracts
            };

            return StatusCode(response.Code, response);
        }
        [HttpGet("Get-Contract-Detail")]
        public async Task<ActionResult> GetContractDetail([FromQuery] string contractId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var contractDetail = await _customerContractService.GetContractDetailByContractId(token, contractId);

            if (contractDetail == null)
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Contract not found"
                });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get contract detail successfully",
                Data = contractDetail
            });
        }

    }
}
