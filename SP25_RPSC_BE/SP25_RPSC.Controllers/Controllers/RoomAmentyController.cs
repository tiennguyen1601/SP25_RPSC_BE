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
        private IAmentyService _amentyService;

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
    }
}
