using SP25_RPSC.Data.Models.PostModel.Request;
using SP25_RPSC.Data.Models.PostModel.Response;

namespace SP25_RPSC.Services.Service.PostService
{
    public interface IPostService
    {
        Task<PagedResult<RoommatePostRes>> GetRoommatePosts(string token, RoommatePostSearchReq search);
        Task<RoommatePostDetailRes> GetRoommatePostDetail(string postId);
        Task<RoommatePostRes> CreateRoommatePost(string token, CreateRoommatePostReq request);
        Task<RoommatePostRes> GetPostRoommateByCustomerId(string token);
    }
}
