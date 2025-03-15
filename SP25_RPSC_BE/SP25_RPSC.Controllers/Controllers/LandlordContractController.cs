using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.LandlordContractService;
using SP25_RPSC.Services.Service.UserService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandlordContractController : ControllerBase
    {
        private readonly ILandlordContractService _landlordContractService;

        public LandlordContractController(ILandlordContractService landlordContractService)
        {
            _landlordContractService = landlordContractService;
        }

        [HttpGet]
        [Route("Get-LandlordContract")]
        public async Task<ActionResult> GetAllLandlordContract(int pageIndex, int pageSize, string searchQuery = null, string status = null)
        {
            var customers = await _landlordContractService.GetAllLandlordContract(searchQuery, pageIndex, pageSize, status);
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get LandlordContract successfully",
                Data = customers
            };
            return StatusCode(response.Code, response);
        }

    }
}
