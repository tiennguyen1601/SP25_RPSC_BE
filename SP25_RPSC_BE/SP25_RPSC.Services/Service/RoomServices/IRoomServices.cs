using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.RoomModel.RequestModel;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
using SP25_RPSC.Data.Models.RoomStay;

namespace SP25_RPSC.Services.Service.RoomServices
{
    public interface IRoomServices
    {
        Task<GetRequiresRoomRentalByLandlordResponseModel> GetRequiresRoomRentalByLandlordId(string token, string searchQuery, int pageIndex, int pageSize);
        Task<RoomCountResponseModel> GetRoomCountsByLandlordId(string token);

        Task<bool> CreateRoom(RoomCreateRequestModel model);
        Task<GetRoomByRoomTypeIdResponseModel> GetRoomDetailByRoomId(string roomId);
        Task<GetRoomByRoomTypeIdResponseModel> GetRoomByRoomTypeId(string roomTypeId, int pageIndex, int pageSize, string searchQuery = "", string status = null);
        Task<(List<RoomResponseModel> Rooms, int TotalPosts, int TotalRooms)> GetAllRoomsAsync(
    decimal? minPrice = null,
    decimal? maxPrice = null,
    string roomTypeName = null,
    string district = null,
    List<string> amenityIds = null,
    int pageIndex = 1,
    int pageSize = 10);
        Task<List<FeedbackResponseModel>> GetFeedbacksByRoomIdAsync(string rentalRoomId);
        Task<RoomDetailResponseModel> GetRoomDetailByIdAsync(string roomId);
        Task<List<UserPastRoomRes>> GetUserPastRooms(string token);
        Task<List<RoomDto>> GetRoomsByLandlordAsync(string token, int pageNumber, int pageSize);
        Task<bool> UpdateRoom(string roomId, RoomUpdateRequestModel model, string token);
        Task<bool> InactiveRoom (string roomId, string token);
        Task<bool> CreatePostRoom(string token, PostRoomCreateRequestModel model);
        //Task<bool> UpdatePostRoom(string token, PostRoomUpdateRequestModel model);
        Task<PostRoomDetailResponseModel> GetPostRoomById(string postRoomId);
        Task<List<RoomAvailableDto>> GetAvailableRoomsByLandlordAsync(string token);
    }
}
