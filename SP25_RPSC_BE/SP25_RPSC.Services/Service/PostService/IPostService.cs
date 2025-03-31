using SP25_RPSC.Data.Models.PostModel.Response;
using SP25_RPSC.Data.Models.PostModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PostService
{
    public interface IPostService
    {
        Task<PagedResult<RoommatePostRes>> GetRoommatePosts(RoommatePostSearchReq search);
        Task<RoommatePostDetailRes> GetRoommatePostDetail(string postId);
        Task<RoommatePostRes> CreateRoommatePost(string token, CreateRoommatePostReq request);
    }
}
