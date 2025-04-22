using SP25_RPSC.Data.Models.FeedbackModel.Response;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Data.Models.FeedbackModel.Request;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Data.Enums;

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
                Type = FeedbackTypeEnums.Room.ToString(),
                Status = StatusEnums.Active.ToString(),
                Rating = model.Rating,
                ReviewerId = userId,
                RentalRoomId = model.RentalRoomId,
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

        public async Task<bool> CreateFeedBackRoommate(FeedBackRoommateRequestModel request, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var reviewer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (reviewer == null)
            {
                throw new UnauthorizedAccessException("Reviewer not found.");
            }

            if (string.IsNullOrEmpty(request.RevieweeId))
            {
                throw new InvalidOperationException("Reviewee ID is required.");
            }
            var revieweeUserId = request.RevieweeId;

            var reviewee = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == revieweeUserId,
                includeProperties: "User")).FirstOrDefault();
            if (reviewer == null)
            {
                throw new KeyNotFoundException("Reviewee not found.");
            }

            var feedbackCheck = (await _unitOfWork.FeedbackRepository.Get(
                filter: fb => fb.ReviewerId == userId && 
                        fb.RevieweeId == reviewee.User.UserId &&
                        fb.Status.Equals(StatusEnums.Active.ToString()))).FirstOrDefault();
            if (feedbackCheck != null)
            {
                throw new InvalidOperationException("You have already given feedback about this roommate.");
            }


            var sharedRoomStays = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == reviewer.CustomerId,
                includeProperties: "RoomStay"
            );

            var sharedRoomStayIds = sharedRoomStays
                .Select(rsc => rsc.RoomStayId)
                .ToList();

            var revieweeInSharedRoomStays = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == reviewee.CustomerId &&
                              sharedRoomStayIds.Contains(rsc.RoomStayId)
            );

            if (!revieweeInSharedRoomStays.Any())
            {
                throw new InvalidOperationException("You are not roommate.");
            }


            // Check xem co dang o cung nhau hay khong
            var currentReviewerRoomStays = await _unitOfWork.RoomStayCustomerRepository.Get(
                       filter: rsc => rsc.CustomerId == reviewer.CustomerId &&
                                     (rsc.Status == "Active" || rsc.Status == "Pending"),
                       includeProperties: "RoomStay"
                   );

            var currentReviewerRoomStayIds = currentReviewerRoomStays
                .Select(rsc => rsc.RoomStayId)
                .ToList();

            var currentlySharing = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == reviewee.CustomerId &&
                              currentReviewerRoomStayIds.Contains(rsc.RoomStayId) &&
                              (rsc.Status == "Active" || rsc.Status == "Pending")
            );

            if (currentlySharing.Any())
            {
                throw new InvalidOperationException("You are still in sharing room.");
            }

            var feedback = new Feedback()
            {
                FeedbackId = Guid.NewGuid().ToString(),
                Description = request.Description,
                Type = FeedbackTypeEnums.Roommate.ToString(),
                Status = StatusEnums.Active.ToString(),
                Rating = request.Rating,
                RevieweeId = request.RevieweeId,
                ReviewerId = userId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(request.Images);
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


        public async Task<bool> UpdateFeedbackRoom(string feedbackId, UpdateFeedbackRoomRequestModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);

            if (feedback == null)
            {
                return false;
            }

            if (feedback.ReviewerId != userId)
            {
                return false;
            }

            if ((DateTime.Now - feedback.CreatedDate.Value).TotalDays > 3)
            {
                return false;
            }

            feedback.Description = model.Description;
            feedback.Rating = model.Rating;
            feedback.UpdatedDate = DateTime.Now;

            if (model.Images != null && model.Images.Any())
            {
                var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(model.Images);

                var existingImages = await _unitOfWork.ImageRfRepository.Get(filter: img => img.FeedbackId == feedbackId);
                foreach (var image in existingImages)
                {
                    await _unitOfWork.ImageRfRepository.Delete(image);
                }

                foreach (var link in downloadUrl)
                {
                    var image = new ImageRf
                    {
                        ImageRfid = Guid.NewGuid().ToString(),
                        ImageRfurl = link,
                        FeedbackId = feedbackId
                    };
                    await _unitOfWork.ImageRfRepository.Add(image);
                }
            }

            await _unitOfWork.FeedbackRepository.Update(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteFeedbackRoom(string feedbackId, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);

            if (feedback == null)
            {
                return false;
            }

            if (feedback.ReviewerId != userId)
            {
                return false;
            }

            if ((DateTime.Now - feedback.CreatedDate.Value).TotalDays > 3)
            {
                return false;
            }

            var images = await _unitOfWork.ImageRfRepository.Get(filter: img => img.FeedbackId == feedbackId);
            foreach (var image in images)
            {
                await _unitOfWork.ImageRfRepository.Delete(image);
            }

            await _unitOfWork.FeedbackRepository.Delete(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateFeedbackRoommate(UpdateFeedbackRoommateReq request, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(request.FeedbackId);
            if (feedback == null)
            {
                throw new KeyNotFoundException("Feedback not found.");
            }

            if (feedback.ReviewerId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this feedback.");
            }

            var fbCreatedDate = feedback.CreatedDate;
            var nowDate = DateTime.Now;
            var checkDate = nowDate - fbCreatedDate;

            if (checkDate.HasValue && checkDate.Value.TotalDays > 3)
            {
                throw new InvalidOperationException("Feedback can only be updated within 3 days of creation.");
            }

            feedback.Description = request.Description;
            feedback.Rating = request.Rating;
            feedback.UpdatedDate = DateTime.Now;

            // Handle image updates if provided
            if (request.Images != null && request.Images.Any())
            {
                var existingImages = await _unitOfWork.ImageRfRepository.Get(
                    filter: i => i.FeedbackId == feedback.FeedbackId
                );

                foreach (var image in existingImages)
                {
                    _unitOfWork.ImageRfRepository.Delete(image);
                }

                var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(request.Images);
                foreach (var link in downloadUrl)
                {
                    var image = new ImageRf
                    {
                        ImageRfid = Guid.NewGuid().ToString(),
                        ImageRfurl = link,
                        FeedbackId = feedback.FeedbackId
                    };
                    await _unitOfWork.ImageRfRepository.Add(image);
                }
            }

            _unitOfWork.FeedbackRepository.Update(feedback);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteFeedbackRoommate(string feedbackId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);
            if (feedback == null)
            {
                throw new KeyNotFoundException("Feedback not found.");
            }

            if (feedback.ReviewerId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this feedback.");
            }

            var fbCreatedDate = feedback.CreatedDate;
            var nowDate = DateTime.Now;
            var checkDate = nowDate - fbCreatedDate;

            if (checkDate.HasValue && checkDate.Value.TotalDays > 3)
            {
                throw new InvalidOperationException("Feedback can only be deleted within 3 days of creation.");
            }

            feedback.Status = StatusEnums.Inactive.ToString();
            feedback.UpdatedDate = DateTime.Now;

            _unitOfWork.FeedbackRepository.Update(feedback);
            await _unitOfWork.SaveAsync();

            return true;
        }


    }
}
