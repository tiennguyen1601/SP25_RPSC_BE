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

namespace SP25_RPSC.Services.Service.PostService
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IEmailService _emailService;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryStorageService cloudinaryStorageService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryStorageService = cloudinaryStorageService;
            _emailService = emailService;
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
                    RoomNumber = post.RentalRoom.RoomNumber,
                    Title = post.RentalRoom.Title,
                    Description = post.RentalRoom.Description,
                    Location = post.RentalRoom.Location,
                    RoomTypeName = post.RentalRoom.RoomType.RoomTypeName,
                    Price = post.RentalRoom.RoomPrices?
                        .Where(rp => rp.ApplicableDate <= DateTime.Now)
                        .OrderByDescending(rp => rp.ApplicableDate)
                        .FirstOrDefault()?.Price,
                    Square = post.RentalRoom.RoomType.Square,
                    Area = post.RentalRoom.RoomType.Area,
                    RoomImages = post.RentalRoom.RoomImages?.Select(ri => ri.ImageUrl).ToList() ?? new List<string>(),
                    RoomAmenities = post.RentalRoom.RoomAmentiesLists?
                                    .Where(ra => ra.RoomAmenty != null)
                                    .Select(ra => ra.RoomAmenty.Name)
                                    .ToList() ?? new List<string>()
                } : null
            };

            return postDetailResponse;
        }

        public async Task<PagedResult<RoommatePostRes>> GetRoommatePosts(RoommatePostSearchReq search)
        {
            search.PageNumber = search.PageNumber <= 0 ? 1 : search.PageNumber;
            search.PageSize = search.PageSize <= 0 ? 10 : search.PageSize;

            var query = await _unitOfWork.PostRepository.Get(
                filter: p => 
                    p.Status == StatusEnums.Pending.ToString() && 
                    p.User != null,
                includeProperties: "User,User.Customers,RentalRoom"
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

    }
}
