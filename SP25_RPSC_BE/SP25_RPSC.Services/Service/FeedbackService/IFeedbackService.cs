using SP25_RPSC.Data.Models.FeedbackModel.Request;
using SP25_RPSC.Data.Models.FeedbackModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.FeedbackService
{
    public interface IFeedbackService
    {
        Task<ViewFeedbackResponseDTO> GetFeedbacksRoomByLandlord(string token, string searchQuery, int pageIndex, int pageSize);
        Task<FeedbackDetailResDTO> GetFeedbackDetailById(string feedbackId);
        Task<MyFeedbackRes> GetMyFeedbacks(string token);
        Task<bool> CreateFeedBackRoom(FeedBackRoomRequestModel model, string token);
        Task<bool> UpdateFeedbackRoom(UpdateFeedbackRoomRequestModel model, string token);
        Task<bool> DeleteFeedbackRoom(string feedbackId, string token);

        Task<bool> CreateFeedBackRoommate(FeedBackRoommateRequestModel request, string token);
        Task<bool> UpdateFeedbackRoommate(UpdateFeedbackRoommateReq request, string token);
        Task<bool> DeleteFeedbackRoommate(string feedbackId, string token);

    }
}
