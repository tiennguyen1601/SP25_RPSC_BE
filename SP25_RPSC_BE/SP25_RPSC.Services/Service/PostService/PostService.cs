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
                includeProperties: "")).FirstOrDefault();
            if (rentalRoom == null)
            {
                throw new ArgumentException("Room not found.");
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
                    // Get the price that was valid at post creation time
                    // (the most recent price that was applicable before or on post creation date)
                    Price = rs.RoomServicePrices?
                        .Where(rsp => rsp.ApplicableDate <= postCreationDate)
                        .OrderByDescending(rsp => rsp.ApplicableDate)
                        .FirstOrDefault()?.Price
                })
                .ToList() ?? new List<RoomServiceInfo>();

            var postDetailResponse = new RoommatePostDetailRes
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Location = post.RentalRoom?.Location,
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
                    Price = post.Price,
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



    }
}
