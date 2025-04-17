using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.Models.UserModels.Request;
using SP25_RPSC.Services.Service.UserService;
using SP25_RPSC.Services.Service.CustomerService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;

        public UserController(IUserService userService, ICustomerService customerService)
        {
            _userService = userService;
            _customerService = customerService;
        }

        [HttpGet]
        [Route("Get-Landlord")]
        public async Task<ActionResult> GetAllLanlord(int pageIndex, int pageSize, string searchQuery = null, string status = null)
        {
            var landlords = await _userService.GetAllLandLord(searchQuery, pageIndex, pageSize, status);
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
        public async Task<ActionResult> GetAllCustomer(int pageIndex, int pageSize, string searchQuery = null, string status = null)
        {
            var customers = await _userService.GetAllCustomer(searchQuery, pageIndex, pageSize, status);
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Customer successfully",
                Data = customers
            };
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Route("Register-Landlord")]
        public async Task<IActionResult> RegisterLandlord([FromForm] LandlordRegisterReqModel createRequestModel, string email)
        {
            await _userService.RegisterLandlord(createRequestModel, email);
            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Register successfully",
            };

            return StatusCode(response.Code, response);
        }


        [HttpGet]
        [Route("Get-Landlord-Regis")]
        public async Task<ActionResult> GetLanlordRegis(int pageIndex, int pageSize, string searchQuery = null)
        {
            var landlords = await _userService.GetRegisLandLord(searchQuery, pageIndex, pageSize);
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
        [Route("Get-Landlord-By-Id")]
        public async Task<ActionResult> GetLandlordById([FromQuery] string landlordId)
        {
            var landlord = await _userService.GetRegisLandLordById(landlordId);

            if (landlord == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Landlord not found."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Landlord successfully.",
                Data = landlord
            });
        }

        [HttpPut]
        [Route("Update-Landlord-Status")]
        public async Task<IActionResult> UpdateLandlordStatus(
                                                                [FromQuery] string landlordId,
                                                                [FromQuery] bool isApproved,
                                                                [FromQuery] string rejectionReason = "")
        {
            bool isUpdated = await _userService.UpdateLandlordStatus(landlordId, isApproved, rejectionReason);

            if (!isUpdated)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to update landlord status."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Landlord status updated successfully."
            });
        }

        [HttpGet]
        [Route("Get-profile-Landlord-By-Id")]
        public async Task<ActionResult> GetLandlordProdfileById([FromQuery] string landlordId)
        {
            var landlord = await _userService.GetProfileLordById(landlordId);

            if (landlord == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Landlord not found."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get Landlord successfully.",
                Data = landlord
            });
        }
                [HttpPut]
        [Route("Update-Landlord-Profile")]
        public async Task<IActionResult> UpdateLandlordProfile([FromForm] UpdateLandlordProfileRequest updateRequestModel, [FromQuery] string landlordId)
        {
            bool isUpdated = await _userService.UpdateLandlordProfile(landlordId, updateRequestModel);

            if (!isUpdated)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to update landlord profile."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Landlord profile updated successfully."
            });
        }

        [HttpGet]
        [Route("Get-Total-Users")]
        public async Task<ActionResult> GetTotalUsers()
        {
            var (totalCustomers, totalLandlords) = await _userService.GetTotalUserCounts();

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get total users successfully",
                Data = new
                {
                    TotalCustomers = totalCustomers,
                    TotalLandlords = totalLandlords
                }
            };

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        [Route("Update-Customer-Profile")]
        public async Task<IActionResult> UpdateCustomerProfile([FromForm] UpdateInfoReq updateReqtModel, [FromQuery] string userEmail)
        {
            bool isUpdated = await _customerService.UpdateInfo(updateReqtModel, userEmail);

            if (!isUpdated)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to update customer profile."
                });
            }

            return StatusCode((int)HttpStatusCode.OK, new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Customer profile updated successfully."
            });
        }
        [HttpGet]
        [Route("Get-Customer-By-UserId")]
        public async Task<IActionResult> GetCustomerByUserId()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _userService.GetCustomerByUserId(token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Customer information retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving customer information: " + ex.Message
                });
            }
        }

        [HttpPut]
        [Route("Edit-Customer-Profile")]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerRequestModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _userService.UpdateCustomer(token, model);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Customer profile updated successfully."
                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Failed to update customer profile."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while updating customer profile: " + ex.Message
                });
            }
        }


        [HttpPut]
        [Route("Update-User-Profile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserRequestModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _userService.UpdateUser(token, model);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "User profile updated successfully."
                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Failed to update user profile."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while updating user profile: " + ex.Message
                });
            }
        }


        [HttpGet]
        [Route("Get-Landlord-By-UserId")]
        public async Task<IActionResult> GetLandlordByUserId()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _userService.GetLandlordByUserId(token);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Landlord information retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving landlord information: " + ex.Message
                });
            }
        }

        [HttpPut]
        [Route("Edit-Landlord-Profile")]
        public async Task<IActionResult> UpdateLandlordProfile([FromBody] UpdateLandlordRequestModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _userService.UpdateLandlord(token, model);

                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Landlord profile updated successfully."
                    });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Failed to update landlord profile."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while updating landlord profile: " + ex.Message
                });
            }
        }



    }
}
