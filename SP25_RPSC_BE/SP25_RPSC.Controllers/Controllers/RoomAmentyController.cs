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
    }
}
