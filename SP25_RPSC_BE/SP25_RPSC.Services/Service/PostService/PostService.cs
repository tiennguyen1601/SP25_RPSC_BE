using AutoMapper;
using SP25_RPSC.Data.Models.PostModel.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PostModel.Request;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using Microsoft.Extensions.Hosting;

namespace SP25_RPSC.Services.Service.PostService
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;
        private readonly IDecodeTokenHandler _decodeTokenHandler;


        public PostService(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<RoommatePostRes> CreateRoommatePost(string token, CreateRoommatePostReq request)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(filter: c => c.UserId == userId,
                    includeProperties: "User")).FirstOrDefault();
            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var rentalRoom = (await _unitOfWork.RoomRepository.Get(filter: r => r.RoomId == request.RentalRoomId,
                includeProperties: "RoomType")).FirstOrDefault();
            if (rentalRoom == null)
            {
                throw new ArgumentException("Room not found.");
            }
            if (rentalRoom.RoomType == null) {
                throw new ArgumentException("RoomType not found");
            }

            // lay gia phong
            var currentRoomPrice = rentalRoom.RoomPrices
                   .Where(rp => rp.ApplicableDate <= DateTime.Now)
                   .OrderByDescending(rp => rp.ApplicableDate)
                   .FirstOrDefault();

            if (currentRoomPrice == null || !currentRoomPrice.Price.HasValue)
            {
                throw new ArgumentException("Room price information not found or invalid");
            }

            // gia sharing kh dc > gia phong
            if (request.Price > currentRoomPrice.Price)
            {
                throw new InvalidOperationException($"Sharing price ({request.Price}) cannot exceed the room's price ({currentRoomPrice.Price}).");
            }

            var roomStayCustomers = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == customer.CustomerId &&
                               rsc.RoomStay.RoomId == request.RentalRoomId &&
                               rsc.Type == CustomerTypeEnums.Tenant.ToString() &&
                               rsc.Status == StatusEnums.Active.ToString() &&
                               rsc.RoomStay.Status == StatusEnums.Active.ToString() &&
                               (rsc.RoomStay.EndDate == null || rsc.RoomStay.EndDate > DateTime.Now)
            );

            var isRoomTenant = roomStayCustomers.Any();
            if (!isRoomTenant)
            {
                throw new UnauthorizedAccessException("You must be tenant in this room to create a roommate post.");
            }

            var currentRoomStay = (await _unitOfWork.RoomStayRepository.Get(
                    filter: rs => rs.RoomId == request.RentalRoomId &&
                                 rs.Status == StatusEnums.Active.ToString() &&
                                 (rs.EndDate == null || rs.EndDate > DateTime.Now))).FirstOrDefault();
            if (currentRoomStay == null)
            {
                throw new ArgumentException("RoomStay not found");
            }

            var maxOccupancy = rentalRoom.RoomType.MaxOccupancy ?? 0;

            var currentOccupants = await _unitOfWork.RoomStayCustomerRepository.Get(
                        filter: rsc => rsc.RoomStayId == currentRoomStay.RoomStayId &&
                                      rsc.Status == StatusEnums.Active.ToString());
            int currentOccupantCount = currentOccupants.Count();

            if (currentOccupantCount >= maxOccupancy)
            {
                throw new InvalidOperationException($"This room has reached its maximum occupancy of {maxOccupancy} people. You cannot post for this room.");
            }

            var existingPosts = await _unitOfWork.PostRepository.Get(
                        filter: p => p.UserId == userId &&
                                     p.RentalRoomId == request.RentalRoomId &&
                                     p.Status == StatusEnums.Active.ToString());
            if (existingPosts.Any())
            {
                throw new InvalidOperationException("You already have an active roommate post for this room.");
            }

            var newPost = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                Title = request.Title,
                Description = request.Description,
                Status = StatusEnums.Active.ToString(), 
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                RentalRoomId = request.RentalRoomId,
                Price = request.Price 
            };

            await _unitOfWork.PostRepository.Add(newPost);
            await _unitOfWork.SaveAsync();

            var postOwnerInfo = new PostOwnerInfo
            {
                UserId = userId,
                FullName = customer.User.FullName,
                Avatar = customer.User.Avatar,
                Gender = customer.User.Gender,
                Age = customer.User?.Dob.HasValue == true ? CalculateAge(customer.User.Dob.Value) : null,
                LifeStyle = customer.LifeStyle,
                Requirement = customer.Requirement,
                PostOwnerType = customer.CustomerType
            };

            var response = new RoommatePostRes
            {
                PostId = newPost.PostId,
                Title = newPost.Title,
                Description = newPost.Description,
                Location = rentalRoom.Location, 
                Price = newPost.Price,
                Status = newPost.Status,
                CreatedAt = newPost.CreatedAt,
                PostOwnerInfo = postOwnerInfo
            };

            return response;
        }

        public async Task<RoommatePostDetailRes> GetRoommatePostDetail(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                throw new ArgumentException("Post ID cannot be null or empty.", nameof(postId));
            }

            var post = await _unitOfWork.PostRepository.GetById(postId);
            if (post == null) {
                throw new KeyNotFoundException($"Post with ID {postId} not found.");
            }

            int totalRoomers = post.RentalRoom.RoomStays
                    .SelectMany(rs => rs.RoomStayCustomers)
                    .Count();

            // Get room services with prices that were valid at the time the post was created
            var postCreationDate = post.CreatedAt ?? DateTime.Now;
            var roomServices = post.RentalRoom?.RoomType?.RoomServices?
    .Where(rs => rs.Status.Equals(StatusEnums.Active.ToString()))
    .Select(rs => new RoomServiceInfo
    {
        ServiceId = rs.RoomServiceId,
        ServiceName = rs.RoomServiceName,
        Description = rs.Description,
        // Lấy giá dịch vụ hợp lệ, nếu có
        Price = rs.RoomServicePrices?
            .Where(rsp => rsp.ApplicableDate <= postCreationDate || rsp.ApplicableDate == null) // Check if ApplicableDate is NULL or before the post creation date
            .OrderByDescending(rsp => rsp.ApplicableDate ?? DateTime.MinValue) // Order by ApplicableDate, using DateTime.MinValue if null
            .FirstOrDefault()?.Price
    })
    .ToList() ?? new List<RoomServiceInfo>();


            var roomPrice = post.RentalRoom.RoomPrices?
                .Where(rp => rp.ApplicableDate <= postCreationDate)
                .OrderByDescending(rp => rp.ApplicableDate)
                .FirstOrDefault()?.Price ?? 0;

            var postDetailResponse = new RoommatePostDetailRes
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Location = post.RentalRoom?.Location,
                PriceShare = post.Price,
                Status = post.Status,
                CreatedAt = post.CreatedAt,
                PostOwnerInfo = new PostOwnerInfo
                {
                    FullName = post.User?.FullName,
                    UserId = post.User.UserId,
                    Avatar = post.User?.Avatar,
                    Gender = post.User?.Gender,
                    Age = post.User?.Dob.HasValue == true ? CalculateAge(post.User.Dob.Value) : null,
                    LifeStyle = post.User?.Customers?.FirstOrDefault()?.LifeStyle,
                    Requirement = post.User?.Customers?.FirstOrDefault()?.Requirement,
                    PostOwnerType = post.User?.Customers?.FirstOrDefault()?.CustomerType
                },
                RoomInfo = post.RentalRoom != null ? new RoomInfo
                {
                    RoomId = post.RentalRoom.RoomId,
                    LandlordName = post.RentalRoom.RoomType?.Landlord?.User?.FullName ?? "Unknown",
                    RoomNumber = post.RentalRoom.RoomNumber,
                    Title = post.RentalRoom.Title,
                    Description = post.RentalRoom.Description,
                    Location = post.RentalRoom.Location,
                    RoomTypeName = post.RentalRoom.RoomType.RoomTypeName,
                    Price = roomPrice, // Su dung gia phong tai thoi diem dang bai
                    Square = post.RentalRoom.RoomType.Square,
                    Area = post.RentalRoom.RoomType.Area,
                    TotalRoomer = totalRoomers,
                    RoomImages = post.RentalRoom.RoomImages?.Select(ri => ri.ImageUrl).ToList() ?? new List<string>(),
                    RoomAmenities = post.RentalRoom.RoomAmentiesLists?
                                        .Where(ra => ra.RoomAmenty != null)
                                        .Select(ra => ra.RoomAmenty.Name)
                                        .ToList() ?? new List<string>(),
                    Services = roomServices
                } : null
            };
            return postDetailResponse;
        }

        public async Task<PagedResult<RoommatePostRes>> GetRoommatePosts(string token, RoommatePostSearchReq search)
        {
            search.PageNumber = search.PageNumber <= 0 ? 1 : search.PageNumber;
            search.PageSize = search.PageSize <= 0 ? 10 : search.PageSize;

            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var query = await _unitOfWork.PostRepository.Get(
                filter: p => 
                    p.Status == StatusEnums.Active.ToString() && 
                    p.User != null &&
                    p.UserId != userId,
                includeProperties: "User,User.Customers,RentalRoom,RentalRoom.RoomPrices"
            );
            
            if (!string.IsNullOrWhiteSpace(search.Address))
            {
                query = query.Where(p =>
                    p.RentalRoom != null &&
                    p.RentalRoom.Location != null &&
                    p.RentalRoom.Location.Contains(search.Address));
            }

            if (search.MinBudget.HasValue)
            {
                query = query.Where(p => p.Price >= search.MinBudget.Value);
            }
            if (search.MaxBudget.HasValue)
            {
                query = query.Where(p => p.Price <= search.MaxBudget.Value);
            }

            var filteredQuery = query
                .Select(p => new
                {
                    Post = p,
                    User = p.User,
                    Customer = p.User.Customers.FirstOrDefault()
                })
                .Where(x => x.User != null && x.Customer != null);

            if (search.MinAge.HasValue || search.MaxAge.HasValue)
            {
                filteredQuery = filteredQuery.Where(x =>
                    x.User.Dob.HasValue &&
                    ((!search.MinAge.HasValue) ||
                     (DateTime.Today.Year - x.User.Dob.Value.Year >= search.MinAge.Value)) &&
                    ((!search.MaxAge.HasValue) ||
                     (DateTime.Today.Year - x.User.Dob.Value.Year <= search.MaxAge.Value))
                );
            }

            if (!string.IsNullOrWhiteSpace(search.Gender))
            {
                filteredQuery = filteredQuery.Where(x => x.User.Gender == search.Gender);
            }

            if (search.LifeStyles != null && search.LifeStyles.Any())
            {
                filteredQuery = filteredQuery.Where(x =>
                    search.LifeStyles.Contains(x.Customer.LifeStyle));
            }

            if (search.CustomerTypes != null && search.CustomerTypes.Any())
            {
                filteredQuery = filteredQuery.Where(x =>
                    search.CustomerTypes.Contains(x.Customer.CustomerType));
            }

            int totalCount = filteredQuery.Count();

            var pagedPosts = filteredQuery
                .OrderByDescending(x => x.Post.CreatedAt)
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var postResponses = pagedPosts.Select(x => new RoommatePostRes
            {
                PostId = x.Post.PostId,
                Title = x.Post.Title,
                Description = x.Post.Description,
                Price = x.Post.Price,
                Status = x.Post.Status,
                CreatedAt = x.Post.CreatedAt,
                Location = x.Post.RentalRoom?.Location,
                PostOwnerInfo = new PostOwnerInfo
                {
                    FullName = x.User.FullName,
                    Avatar = x.User.Avatar,
                    Gender = x.User.Gender,
                    Age = x.User.Dob.HasValue ? CalculateAge(x.User.Dob.Value) : null,
                    LifeStyle = x.Customer.LifeStyle,
                    Requirement = x.Customer.Requirement,
                    PostOwnerType = x.Customer.CustomerType
                }
            }).ToList();

            return new PagedResult<RoommatePostRes>
            {
                Items = postResponses,
                TotalCount = totalCount,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize
            };
        }

        private int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dateOfBirth.Year;

            if (today.Month < dateOfBirth.Month ||
                (today.Month == dateOfBirth.Month && today.Day < dateOfBirth.Day))
            {
                age--;
            }

            return age;
        }
        public async Task<RoommatePostRes> GetPostRoommateByCustomerId(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var customer = (await _unitOfWork.CustomerRepository.Get(
                filter: c => c.UserId == userId,
                includeProperties: "User" // Đảm bảo thông tin User được bao gồm
            )).FirstOrDefault();

            if (customer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var post = (await _unitOfWork.PostRepository.Get(
                filter: p => p.UserId == userId && p.Status == StatusEnums.Active.ToString(),
                orderBy: p => p.OrderByDescending(c => c.CreatedAt)
            )).FirstOrDefault();

            if (post == null)
            {
                throw new KeyNotFoundException($"No active post found for the customer with ID {userId}.");
            }


            var customerInfo = new
            {
                customerName = customer.User?.FullName,
                customerEmail = customer.User?.Email,
                customerPhoneNumber = customer.User?.PhoneNumber
            };

            var postDetailResponse = new RoommatePostRes
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Location = post.RentalRoom?.Location,
                Price = post.Price,
                Status = post.Status,
                CreatedAt = post.CreatedAt,
                CustomerName = customerInfo.customerName,
                CustomerEmail = customerInfo.customerEmail,
                CustomerPhoneNumber = customerInfo.customerPhoneNumber
            };

            return postDetailResponse;
        }

        public async Task<RoommatePostRes> UpdateRoommatePost(string token, string postId, UpdateRoommatePostReq request)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var post = await _unitOfWork.PostRepository.GetByIDAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {postId} not found.");
            }

            // Check if the user is the owner of the post
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this post.");
            }

            // Check if post has any pending or active requests
            //var hasRequests = await _unitOfWork.RequestRepository.Exists(
            //    r => r.PostId == postId &&
            //         (r.Status == StatusEnums.Pending.ToString() || r.Status == StatusEnums.Active.ToString())
            //);

            var hasRequests = await _unitOfWork.RoommateRequestRepository.GetRoommateRequestsByPostId(postId);

            if (hasRequests.Count > 0)
            {
                throw new InvalidOperationException("This post cannot be updated because it has pending or active requests.");
            }

            // Update post properties
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                post.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                post.Description = request.Description;
            }

            //if (request.Price.HasValue && request.Price.Value > 0)
            //{
            //    post.Price = request.Price.Value;
            //}

            post.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveAsync();

            // Fetch updated post with relations for response
            var updatedPost = await _unitOfWork.PostRepository.Get(
                filter: p => p.PostId == postId,
                includeProperties: "User,User.Customers,RentalRoom"
            );

            var postData = updatedPost.FirstOrDefault();
            var customer = postData.User?.Customers?.FirstOrDefault();

            var response = new RoommatePostRes
            {
                PostId = postData.PostId,
                Title = postData.Title,
                Description = postData.Description,
                Location = postData.RentalRoom?.Location,
                Price = postData.Price,
                Status = postData.Status,
                CreatedAt = postData.CreatedAt,
                PostOwnerInfo = new PostOwnerInfo
                {
                    UserId = postData.UserId,
                    FullName = postData.User?.FullName,
                    Avatar = postData.User?.Avatar,
                    Gender = postData.User?.Gender,
                    Age = postData.User?.Dob.HasValue == true ? CalculateAge(postData.User.Dob.Value) : null,
                    LifeStyle = customer?.LifeStyle,
                    Requirement = customer?.Requirement,
                    PostOwnerType = customer?.CustomerType
                }
            };

            return response;
        }

        public async Task<bool> InactivateRoommatePost(string token, string postId)
        {
            if (token == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var post = await _unitOfWork.PostRepository.GetById(postId);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {postId} not found.");
            }

            // Check if the user is the owner of the post
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to inactivate this post.");
            }

            // Check if post has any pending or active requests

            var hasRequests = await _unitOfWork.RoommateRequestRepository.GetRoommateRequestsByPostId(postId);

            if (hasRequests.Count > 0)
            {
                throw new InvalidOperationException("This post cannot be inactivated because it has pending or active requests.");
            }

            // Update post status to inactive
            post.Status = StatusEnums.Inactive.ToString();
            post.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
