using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.UserService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("Get-Landlord")]
        public async Task<ActionResult<IEnumerable<Landlord>>> GetAllLanlord()
        {
            var landlords = await _userService.GetAllLandLord();
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Lanlord successfully",
                Data = landlords
            };
            return StatusCode(response.Code, response);
        }

        [HttpGet]
        [Route("Get-Customer")]
        public async Task<ActionResult<IEnumerable<Landlord>>> GetAllCustomer()
        {
            var customers = await _userService.GetAllCustomer();
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Lanlord successfully",
                Data = customers
            };
            return StatusCode(response.Code, response);
        }
    }
}
