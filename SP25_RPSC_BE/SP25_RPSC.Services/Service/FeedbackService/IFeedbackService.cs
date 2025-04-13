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

        Task<bool> CreateFeedBackRoom(FeedBackRoomRequestModel model, string token);

        Task<bool> CreateFeedBackCustomer(FeedBackCustomerRequestModel model, string token);
    }
}
