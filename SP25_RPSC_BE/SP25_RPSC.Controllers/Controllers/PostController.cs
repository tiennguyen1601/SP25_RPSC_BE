using Microsoft.AspNetCore.Mvc;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PostModel.Request;
using SP25_RPSC.Data.Models.PostModel.Response;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Services.Service.PostService;
using System.Net;
using System.Threading.Tasks;

namespace SP25_RPSC.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [Route("recommended-roommate-posts")]
        public async Task<IActionResult> GetRecommendedRoommatePosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postService.GetRecommendedRoommatePosts(token, pageNumber, pageSize);

                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = result,
                    Message = "Recommended roommate posts retrieved successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving recommended roommate posts."
                });
            }
        }

        [HttpGet("Get-All-Roommate-Post")]
        public async Task<ActionResult<PagedResult<RoommatePostRes>>> GetAllRoommatePosts([FromQuery] RoommatePostSearchReq searchRequest)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

                searchRequest.PageNumber = searchRequest.PageNumber <= 0 ? 1 : searchRequest.PageNumber;
                searchRequest.PageSize = searchRequest.PageSize <= 0 ? 10 :
                    searchRequest.PageSize > 100 ? 100 : searchRequest.PageSize;

                var result = await _postService.GetRoommatePosts(token, searchRequest);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get roommate posts successfully",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while get roommate posts",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("Get-Roommate-Post-Detail")]
        public async Task<ActionResult<RoommatePostDetailRes>> GetRoommatePostDetail(string postId)
        {
            try
            {
                var result = await _postService.GetRoommatePostDetail(postId);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get roommate post detail successfully",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while get roommate post detail",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("create-roommate-post")]
        public async Task<IActionResult> CreateRoommatePost([FromBody] CreateRoommatePostReq request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            try
            {
                var result = await _postService.CreateRoommatePost(token, request);
                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Create new roommate post successfully",
                    Data = result
                };
                return StatusCode(response.Code, response);
            } catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while create new roommate post",
                    Error = ex.Message
                });
            }

        }

        [HttpGet("Get-Post-Roommate-By-CustomerId")]
        public async Task<ActionResult<RoommatePostDetailRes>> GetPostRoommateByCustomerId()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            try
            {
                var result = await _postService.GetPostRoommateByCustomer(token);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get roommate post by customer ID successfully",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while getting roommate post by customer ID",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("Get-All-Roommate-Post-By-Customer")]
        public async Task<ActionResult<RoommatePostDetailRes>> GetAllPostRoommateByCustomer()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            try
            {
                var result = await _postService.GetAllPostRoommateByCustomer(token);

                ResultModel response = new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Get all roommate post by customer successfully",
                    Data = result
                };
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while getting roommate post by customer",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("update-roommate-post/{postId}")]
        public async Task<IActionResult> UpdateRoommatePost(string postId, [FromBody] UpdateRoommatePostReq request)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postService.UpdateRoommatePost(token, postId, request);

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Roommate post updated successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while getting roommate post by customer ID",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("posts/landlord/customer-roommate")]
        public async Task<IActionResult> GetCustomerRoommatePosts()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _postService.GetPostsByLandlordAsync(token);
            return Ok(result);
        }

        [HttpPut("inactivate-roommate-post-by-tenant/{postId}")]
        public async Task<IActionResult> InactivateRoommatePostByTenant(string postId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postService.InactivateRoommatePostByTenant(token, postId);

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Roommate post has been successfully inactivated.",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while inactivating the roommate post.",
                    Data = ex.Message
                });
            }
        }


        [HttpPut("inactivate-roommate-post-by-landlord/{postId}")]
        public async Task<IActionResult> InactivateRoommatePostByLandlord(string postId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postService.InactivateRoommatePostByLandlord(token, postId);

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Roommate post has been successfully inactivated.",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while inactivating the roommate post.",
                    Data = ex.Message
                });
            }
        }
    }
}
