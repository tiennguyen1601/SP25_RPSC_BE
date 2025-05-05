using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.AmentitiesModel;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.AmentyService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomAmentyController : ControllerBase
    {
        private readonly IAmentyService _amentyService;

        public RoomAmentyController(IAmentyService amentyService)
        {
            _amentyService = amentyService;
        }

        [HttpPost]
        [Route("Create-RoomAmenty")]
        public async Task<ActionResult> CreateRoomAmenty(RoomAmentyRequestCreateModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var amenty = await _amentyService.CreateAmenty(model, token);

            ResultModel response = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "create Amenty successfully",
                Data = amenty
            };

            return StatusCode(response.Code, response);
        }

        [HttpGet("GetAllAmenties")]
        public async Task<ActionResult<GetAllAmentiesResponseModel>> GetAllAmenties(
        [FromQuery] string searchQuery = "",
        [FromQuery] int pageIndex = 1,    
        [FromQuery] int pageSize = 10)  
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _amentyService.GetAllAmentiesByLandlordId(searchQuery, pageIndex, pageSize, token);

            if (result.Amenties == null || !result.Amenties.Any())
            {
                return NotFound(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No amenities found"
                });
            }

            return Ok(new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Amenities retrieved successfully",
                Data = result
            });
        }

        [HttpPut]
        [Route("Update-Amenity/{amenityId}")]
        public async Task<ActionResult> UpdateAmenity(string amenityId, [FromBody] RoomAmentyRequestUpdateModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is required" });
                }

                var result = await _amentyService.UpdateAmenity(model, token, amenityId);
                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Amenity updated successfully"
                    });
                }
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Amenity could not be updated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { isSuccess = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("Delete-Amenity/{amenityId}")]
        public async Task<ActionResult> DeleteAmenity(string amenityId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Authorization token is required" });
                }

                var result = await _amentyService.DeleteAmenity( token, amenityId);
                if (result)
                {
                    return Ok(new ResultModel
                    {
                        IsSuccess = true,
                        Code = (int)HttpStatusCode.OK,
                        Message = "Amenity updated successfully"
                    });
                }
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Amenity could not be updated"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { isSuccess = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
    }
}
