using SP25_RPSC.Data.Models.PostModel.Request;
using SP25_RPSC.Data.Models.PostModel.Response;

namespace SP25_RPSC.Services.Service.PostService
{
    public interface IPostService
    {
        Task<PagedResult<RoommatePostRes>> GetRecommendedRoommatePosts(string token, int pageNumber = 1, int pageSize = 10);
        Task<PagedResult<RoommatePostRes>> GetRoommatePosts(string token, RoommatePostSearchReq search);
        Task<RoommatePostDetailRes> GetRoommatePostDetail(string postId);
        Task<RoommatePostRes> CreateRoommatePost(string token, CreateRoommatePostReq request);
        Task<RoommatePostRes> GetPostRoommateByCustomer(string token);
        Task<List<RoommatePostRes>> GetAllPostRoommateByCustomer(string token);
        Task<RoommatePostRes> UpdateRoommatePost(string token, string postId, UpdateRoommatePostReq request);
        Task<IEnumerable<PostViewModel>> GetPostsByLandlordAsync(string token);
        Task<bool> InactivateRoommatePostByTenant(string token, string postId);
        Task<bool> InactivateRoommatePostByLandlord(string token, string postId);
    }
}
