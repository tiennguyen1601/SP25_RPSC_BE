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
                var result = await _postService.GetPostRoommateByCustomerId(token);

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

        [HttpPut("inactivate-roommate-post/{postId}")]
        public async Task<IActionResult> InactivateRoommatePost(string postId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postService.InactivateRoommatePost(token, postId);

                return StatusCode((int)HttpStatusCode.OK, new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "An error occurred while updating the roommate post."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while inactivating the roommate post",
                    Error = ex.Message
                });
            }
        }
    }
}
