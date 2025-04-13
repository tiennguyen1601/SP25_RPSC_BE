using SP25_RPSC.Data.Models.FeedbackModel.Response;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Data.Models.FeedbackModel.Request;
using Newtonsoft.Json.Linq;

namespace SP25_RPSC.Services.Service.FeedbackService
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler, ICloudinaryStorageService
            cloudinaryStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
            _cloudinaryStorageService = cloudinaryStorageService;
        }


        public async Task<ViewFeedbackResponseDTO> GetFeedbacksRoomByLandlord(string token, string searchQuery, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();
            if (landlord == null)
            {
                return new ViewFeedbackResponseDTO { TotalFeedback = 0, Feebacks = new List<ListFeedbackRoomeRes>() };
            }

            var landlordId = landlord.LandlordId;

            Expression<Func<Room, bool>> roomSearchFilter = r =>
                r.RoomType.LandlordId == landlordId &&
                (string.IsNullOrEmpty(searchQuery) || r.RoomNumber.Contains(searchQuery));

            var rooms = await _unitOfWork.RoomRepository.Get(
                filter: roomSearchFilter,
                includeProperties: "Feedbacks,RoomType,Feedbacks.Reviewer",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalFeedback = await _unitOfWork.RoomRepository.CountAsync(roomSearchFilter);

            var feedbacks = _mapper.Map<List<ListFeedbackRoomeRes>>(
                rooms.SelectMany(r => r.Feedbacks)
            );

            return new ViewFeedbackResponseDTO
            {
                TotalFeedback = totalFeedback,
                Feebacks = feedbacks
            };
        }

        public async Task<FeedbackDetailResDTO> GetFeedbackDetailById(string feedbackId)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);

            if (feedback == null)
            {
                return null;
            }

            var reviewer = await _unitOfWork.UserRepository.GetByIDAsync(feedback.ReviewerId);
            var room = await _unitOfWork.RoomRepository.GetByIDAsync(feedback.RentalRoomId);

            var imageList = await _unitOfWork.ImageRfRepository.Get(filter: img => img.FeedbackId == feedbackId);
            var imageUrls = imageList.Select(img => img.ImageRfurl).Where(url => !string.IsNullOrEmpty(url)).ToList();

            var feedbackDto = _mapper.Map<FeedbackDetailResDTO>(feedback);
            feedbackDto.ImageUrls = imageUrls;

            return feedbackDto;
        }

        public async Task<bool> CreateFeedBackRoom(FeedBackRoomRequestModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = new Feedback()
            {
                FeedbackId = Guid.NewGuid().ToString(),
                Description = model.Description,
                Rating = model.Rating,
                ReviewerId = userId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(model.Images);
            foreach (var link in downloadUrl)
            {
                var Image = new ImageRf
                {
                    ImageRfid = Guid.NewGuid().ToString(),
                    ImageRfurl = link,
                };
                feedback.ImageRves.Add(Image);
            }

            await _unitOfWork.FeedbackRepository.Add(feedback);
            await _unitOfWork.SaveAsync(); 
            return true;
        }

        public async Task<bool> CreateFeedBackCustomer(FeedBackCustomerRequestModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = new Feedback()
            {
                FeedbackId = Guid.NewGuid().ToString(),
                Description = model.Description,
                RevieweeId = model.RevieweeId,
                ReviewerId = userId,
                Rating = model.Rating,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(model.Images);
            foreach (var link in downloadUrl)
            {
                var Image = new ImageRf
                {
                    ImageRfid = Guid.NewGuid().ToString(),
                    ImageRfurl = link,
                };
                feedback.ImageRves.Add(Image);
            }

            await _unitOfWork.FeedbackRepository.Add(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
