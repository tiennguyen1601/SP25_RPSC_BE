using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Models.CustomerModel.Request;
using SP25_RPSC.Data.Models.CustomerModel.Response;
using SP25_RPSC.Services.Service.LandlordService;
using System.Net;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandlordController : Controller
    {
        private readonly ILandlordService _landlordService;

        public LandlordController(ILandlordService landlordService)
        {
            _landlordService = landlordService;
        }

        [HttpGet("tenant-leave-room-requests")]
        public async Task<IActionResult> GetTenantLeaveRoomRequests()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _landlordService.GetListTenantLeaveRoomRequest(token);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("detail-tenant-leave-room-request/{cmoId}")]
        public async Task<IActionResult> GetTenantLeaveRoomRequestDetail(string cmoId)
        {
            try
            {
                var result = await _landlordService.GetDetailTenantLeaveRoomRequest(cmoId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }


        [HttpPut("accept-tenant-leave-room-request/{requestId}")]
        public async Task<IActionResult> AcceptTenantLeaveRoomRequest(string requestId)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _landlordService.AcceptTenantLeaveRoomRequest(token, requestId);
                return Ok(new { success = result, message = "Tenant leave room request has been accepted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("kick-tenant-by-landlord")]
        public async Task<IActionResult> KickTenant([FromBody] KickTenantReq request)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _landlordService.KickTenantbyLanlord(token, request);
                return Ok(new { success = result, message = "Tenant has been removed from the room successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }
    }
}
