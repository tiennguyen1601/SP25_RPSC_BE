using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using MailKit.Search;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.DecodeTokenModel;
using SP25_RPSC.Data.Models.RoomModel.RequestModel;
using SP25_RPSC.Data.Models.RoomModel.RoomResponseModel;
using SP25_RPSC.Data.Models.RoomStay;
using SP25_RPSC.Data.Models.RoomTypeModel.Request;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;

namespace SP25_RPSC.Services.Service.RoomServices
{
    public class RoomServices : IRoomServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;
       private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public RoomServices(IUnitOfWork unitOfWork, IMapper mapper, IDecodeTokenHandler decodeTokenHandler, ICloudinaryStorageService cloudinaryStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
            _cloudinaryStorageService = cloudinaryStorageService;
        }

        public async Task<bool> CreateRoom(RoomCreateRequestModel model)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIDAsync(model.roomtypeId);
            if (roomType == null)
            {
                throw new Exception("RoomType not found."); 
            }

            var landlordId = roomType.LandlordId;

            var activeContract = (await _unitOfWork.LandlordContractRepository.Get(
                contract => contract.LandlordId == landlordId && contract.Status == StatusEnums.Active.ToString()
            )).FirstOrDefault();

            if (activeContract == null)
            {
                throw new Exception("Your Service Package are Expired or you didn't buy Service Package");
            }

            var roomPrice = new List<RoomPrice>
    {
        new RoomPrice
        {
            RoomPriceId = Guid.NewGuid().ToString(),
            Price = model.price,
            ApplicableDate = DateTime.Now,
        }
    };

            var room = new Room
            {
                RoomId = Guid.NewGuid().ToString(),
                RoomNumber = model.RoomNumber,
                //Title = model.Title,
                AvailableDateToRent = model.AvailableDateToRent,
                //Description = model.Description,
                Location = model.Location,
                Status = "Available",
                UpdatedAt = DateTime.Now,
                RoomPrices = roomPrice,
                RoomTypeId = model.roomtypeId,
            };

            var downloadUrl = await _cloudinaryStorageService.UploadImageAsync(model.Images);
            foreach (var link in downloadUrl)
            {
                var image = new RoomImage
                {
                    ImageId = Guid.NewGuid().ToString(),
                    ImageUrl = link,
                };
                room.RoomImages.Add(image);
            }

            foreach (var amenty in model.AmentyId)
            {
                var roomAmentyList = new RoomAmentiesList
                {
                    RoomAmentyId = amenty,
                    RoomId = room.RoomId
                };
                await _unitOfWork.RoomAmentyListRepository.Add(roomAmentyList);
            }
            await _unitOfWork.RoomRepository.Add(room);

            var landlord = await _unitOfWork.LandlordRepository.GetByIDAsync(landlordId);
            if (landlord == null)
            {
                throw new Exception("Landlord not found."); 
            }

            landlord.NumberRoom -= 1;

            if (landlord.NumberRoom == 0)
            {
                activeContract.Status = StatusEnums.Expired.ToString();
                await _unitOfWork.LandlordContractRepository.Update(activeContract);
            }

            await _unitOfWork.LandlordRepository.Update(landlord);

            await _unitOfWork.SaveAsync();

            return true; 
        }




        public async Task<GetRequiresRoomRentalByLandlordResponseModel> GetRequiresRoomRentalByLandlordId(
    string token, string searchQuery, int pageIndex, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId)).FirstOrDefault();

            Expression<Func<Room, bool>> searchFilter = room =>
                room.RoomType != null &&
                room.RoomType.LandlordId == landlord.LandlordId &&
                room.Status == "Available" &&
                room.RoomRentRequests.Any(r => r.Status == "Pending") &&
                (string.IsNullOrEmpty(searchQuery) ||
                 room.RoomNumber.Contains(searchQuery)
                 //room.Title.Contains(searchQuery)
                 );

            var rooms = await _unitOfWork.RoomRepository.Get(
                includeProperties: "RoomType,RoomRentRequests.CustomerRentRoomDetailRequests,RoomImages,RoomPrices",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalRooms = await _unitOfWork.RoomRepository.CountAsync(searchFilter);

            if (rooms == null || !rooms.Any())
            {
                return new GetRequiresRoomRentalByLandlordResponseModel
                {
                    Rooms = new List<ListRoomRes>(),
                    TotalRooms = 0
                };
            }

            var roomResponses = rooms.Select(room => new ListRoomRes
            {
                RoomId = room.RoomId,
                RoomTypeId = room.RoomTypeId,
                RoomRentRequestsId = room.RoomRentRequests.FirstOrDefault()?.RoomRentRequestsId ?? "",
                RoomNumber = room.RoomNumber,
                //Title = room.Title,
                //Description = room.Description,
                Status = room.Status,
                Location = room.Location,
                RoomTypeName = room.RoomType?.RoomTypeName ?? "N/A",
                Square = room.RoomType?.Square,
                Area = room.RoomType?.Area,
                TotalRentRequests = room.RoomRentRequests
                    .Sum(r => r.CustomerRentRoomDetailRequests.Count(c => c.Status == "Pending")),
                RoomImages = room.RoomImages.Select(img => img.ImageUrl).ToList(),
                Price = room.RoomPrices
                    .OrderByDescending(p => p.ApplicableDate)
                    .Select(p => p.Price)
                    .FirstOrDefault()
            }).Where(room => room.TotalRentRequests > 0).ToList();

            return new GetRequiresRoomRentalByLandlordResponseModel
            {
                Rooms = roomResponses,
                TotalRooms = roomResponses.Count > 0 ? totalRooms : 0 
            };
        }



        public async Task<RoomCountResponseModel> GetRoomCountsByLandlordId(string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId))
                            .FirstOrDefault();

            if (landlord == null)
            {
                throw new Exception("Landlord not found");
            }

            var allRooms = (await _unitOfWork.RoomRepository
                        .Get(includeProperties: "RoomType"))
                        .Where(c => c.RoomType != null && c.RoomType.LandlordId == landlord.LandlordId)
                        .ToList();

            var totalRooms = allRooms.Count;
            var totalActiveRooms = allRooms.Count(r => r.Status == "Available");
            var totalRentingRooms = allRooms.Count(r => r.Status == "Renting");

            var totalRequests = await _unitOfWork.CustomerRentRoomDetailRequestRepositories.CountAsync(
    req => req.RoomRentRequests.Room.RoomType != null &&
           req.RoomRentRequests.Room.RoomType.LandlordId == landlord.LandlordId &&
           req.Status == "Pending");


            var totalCustomersRenting = (await _unitOfWork.RoomStayCustomerRepository
                .Get(filter: rsc => rsc.LandlordId == landlord.LandlordId && rsc.Status == "Active"))
                .Count();

            return new RoomCountResponseModel
            {
                TotalRooms = totalRooms,
                TotalRoomsActive = totalActiveRooms,
                TotalRoomsRenting = totalRentingRooms,
                TotalCustomersRenting = totalCustomersRenting,
                TotalRequests = totalRequests
            };
        }
        public async Task<GetRoomByRoomTypeIdResponseModel> GetRoomByRoomTypeId(string roomTypeId, int pageIndex, int pageSize, string searchQuery = "", string status = null)
        {
            Expression<Func<Room, bool>> searchFilter = room =>
                room.RoomTypeId == roomTypeId &&
                (string.IsNullOrEmpty(searchQuery) ||
                 room.RoomNumber.Contains(searchQuery) ||
                 //room.Title.Contains(searchQuery)) &&
                (string.IsNullOrEmpty(status) || room.Status == status));  

            var rooms = await _unitOfWork.RoomRepository.Get(
                includeProperties: "RoomType,RoomImages,RoomPrices,RoomAmentiesLists",
                filter: searchFilter,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var totalRooms = await _unitOfWork.RoomRepository.CountAsync(searchFilter);

            if (rooms == null || !rooms.Any())
            {
                return new GetRoomByRoomTypeIdResponseModel
                {
                    Rooms = new List<ListRoomResByRoomTypeId>(), 
                    TotalRooms = 0
                };
            }

            var roomResponses = _mapper.Map<List<ListRoomResByRoomTypeId>>(rooms);

            return new GetRoomByRoomTypeIdResponseModel
            {
                Rooms = roomResponses,
                TotalRooms = totalRooms
            };
        }
        public async Task<GetRoomByRoomTypeIdResponseModel> GetRoomDetailByRoomId(string roomId)
        {
            var room = (await _unitOfWork.RoomRepository.Get(
                    includeProperties: "RoomType,RoomImages,RoomPrices,RoomAmentiesLists.RoomAmenty", 
                    filter: r => r.RoomId == roomId
                )).FirstOrDefault();

            if (room == null)
            {
                return new GetRoomByRoomTypeIdResponseModel
                {
                    Rooms = new List<ListRoomResByRoomTypeId>() 
                };
            }

            var roomResponse = _mapper.Map<ListRoomResByRoomTypeId>(room);

            return new GetRoomByRoomTypeIdResponseModel
            {
                Rooms = new List<ListRoomResByRoomTypeId> { roomResponse } 
            };
        }

        public async Task<List<RoomResponseModel>> GetAllRoomsAsync(
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string roomTypeName = null,
            string district = null,
            List<string> amenityIds = null)
        {
            var rooms = await _unitOfWork.RoomRepository.GetFilteredRoomsAsync(
                minPrice, maxPrice, roomTypeName, district, amenityIds
            );

            var result = _mapper.Map<List<RoomResponseModel>>(rooms);

            foreach (var room in result)
            {
                var roomEntity = rooms.FirstOrDefault(r => r.RoomId == room.RoomId);
                var contracts = roomEntity?.RoomType?.Landlord?.LandlordContracts;

                if (contracts == null || !contracts.Any())
                {
                    Console.WriteLine($"No contracts found for room {room.RoomId}");
                }

                var activeContract = contracts?
                    .Where(c => c.Status == "Active")
                    .OrderByDescending(c => c.CreatedDate)
                    .FirstOrDefault();

                room.PackageLabel = activeContract?.Package?.Label;
                room.PackagePriorityTime = activeContract?.Package?.PriorityTime;
            }

            return result
                .OrderByDescending(r => r.PackagePriorityTime ?? 0)
                .ThenByDescending(r => r.UpdatedAt)
                .ToList();
        }

        public async Task<RoomDetailResponseModel> GetRoomDetailByIdAsync(string roomId)
        {
            var room = await _unitOfWork.RoomRepository.GetRoomByIdAsync(roomId);

            if (room == null) return null;

            var result = _mapper.Map<RoomDetailResponseModel>(room);

            var activeContract = room.RoomType?.Landlord?.LandlordContracts?
                .Where(c => c.Status == "Active")
                .OrderByDescending(c => c.CreatedDate)
                .FirstOrDefault();

            result.PackageLabel = activeContract?.Package?.Label;
            result.PackagePriorityTime = activeContract?.Package?.PriorityTime;

            result.Landlord = _mapper.Map<LandlordResponseModel>(room.RoomType.Landlord);

            result.RoomServices = room.RoomType.RoomServices.Select(rs => new RoomServiceResponseModel
            {
                RoomServiceId = rs.RoomServiceId,
                RoomServiceName = rs.RoomServiceName,
                Description = rs.Description,
                Status = rs.Status,
                CreatedAt = rs.CreatedAt,
                UpdatedAt = rs.UpdatedAt,
                Prices = rs.RoomServicePrices.Select(p => new RoomServicePriceResponseModel
                {
                    Price = p.Price,
                    ApplicableDate = p.ApplicableDate
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<UserPastRoomRes>> GetUserPastRooms(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var currentCustomer = (await _unitOfWork.CustomerRepository.Get(
                filter: c => c.UserId == userId,
                includeProperties: "User"
            )).FirstOrDefault();

            if (currentCustomer == null)
            {
                throw new UnauthorizedAccessException("Customer not found.");
            }

            var roomStayCustomers = await _unitOfWork.RoomStayCustomerRepository.Get(
                filter: rsc => rsc.CustomerId == currentCustomer.CustomerId,
                includeProperties: "RoomStay,RoomStay.Room,RoomStay.Room.RoomType,RoomStay.Room.RoomImages,RoomStay.Room.RoomType.Address"
            );

            var pastRoomStayCustomers = roomStayCustomers
                .Where(rsc => !rsc.Status.Equals(StatusEnums.Active.ToString()) && !rsc.Status.Equals(StatusEnums.Pending.ToString()))
                .ToList();



            var response = new List<UserPastRoomRes>();

            foreach (var roomStayCustomer in pastRoomStayCustomers)
            {
                var roomStay = roomStayCustomer.RoomStay;
                var room = roomStay?.Room;
                var roomType = room?.RoomType;

                if (roomStay == null || room == null || roomType == null) continue;

                var roomImages = room.RoomImages
                   .Where(ri => !string.IsNullOrEmpty(ri.ImageUrl))
                   .Select(ri => ri.ImageUrl)
                   .ToList();


                var pastRoomInfo = new UserPastRoomRes
                {
                    RoomId = room.RoomId,
                    RoomNumber = room.RoomNumber,
                    //Title = room.Title,
                    //Description = room.Description,
                    RoomTypeName = roomType.RoomTypeName,
                    Address = room.Location,
                    Images = roomImages,
                };

                response.Add(pastRoomInfo);
            }

            return response;
        }

        public async Task<List<RoomDto>> GetRoomsByLandlordAsync(string token, int pageNumber, int pageSize)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId))
                            .FirstOrDefault();

            if (landlord == null)
            {
                throw new Exception("Landlord not found");
            }

            var allRooms = await _unitOfWork.RoomRepository.GetRoomsByLandlordIdAsync(landlord.LandlordId);

            var pagedRooms = allRooms
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var roomDtos = _mapper.Map<List<RoomDto>>(pagedRooms);

            foreach (var dto in roomDtos)
            {
                var room = pagedRooms.First(r => r.RoomId == dto.RoomId);
                var latestPrice = room.RoomPrices
                    .Where(p => p.ApplicableDate <= DateTime.Now)
                    .OrderByDescending(p => p.ApplicableDate)
                    .FirstOrDefault();

                dto.Price = latestPrice?.Price ?? 0;
            }

            return roomDtos;
        }

        public async Task<bool> UpdateRoom(string roomId, RoomUpdateRequestModel model, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId))
                           .FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("User is not a landlord.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomId);
            if (room == null)
            {
                throw new Exception("Room not found.");
            }

            if (landlord.LandlordId != room.RoomType.LandlordId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this room. Only the landlord can update their rooms.");
            }

            if (room.Status == "Renting")
            {
                throw new Exception("Cannot update room that is currently being rented.");
            }

            if(model.RoomTypeId != null)
            {
                var roomType = await _unitOfWork.RoomTypeRepository.GetByIDAsync(model.RoomTypeId);
                if (roomType == null)
                {
                    throw new Exception("RoomType not found.");
                }

                if (roomType.LandlordId != landlord.LandlordId)
                {
                    throw new UnauthorizedAccessException("You can only assign room types that belong to you.");
                }
            }
            if(model.RoomNumber != null)
            {
                room.RoomNumber = model.RoomNumber;
            }
            if (model.Title != null)
            {
                //room.Title = model.Title;
            }
            if (model.AvailableDateToRent != null)
            {
                room.AvailableDateToRent = model.AvailableDateToRent;
            }
            if (model.Description != null)
            {
                //room.Description = model.Description;
            }
            room.UpdatedAt = DateTime.Now;
            room.RoomTypeId = model.RoomTypeId;

            if (model.Price > 0)
            {
                var newRoomPrice = new Data.Entities.RoomPrice
                {
                    RoomPriceId = Guid.NewGuid().ToString(),
                    Price = model.Price,
                    ApplicableDate = DateTime.Now,
                    RoomId = roomId
                };
                await _unitOfWork.RoomPriceRepository.Add(newRoomPrice);
            }

            if (model.Images != null && model.Images.Any())
            {

                var existingImages = (await _unitOfWork.RoomImageRepository.Get(filter: ri => ri.RoomId == roomId)).ToList();
                foreach (var image in existingImages)
                {
                    await _unitOfWork.RoomImageRepository.Remove(image);
                }


                var downloadUrls = await _cloudinaryStorageService.UploadImageAsync(model.Images);
                foreach (var link in downloadUrls)
                {
                    var image = new Data.Entities.RoomImage
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        ImageUrl = link,
                        RoomId = roomId
                    };
                    await _unitOfWork.RoomImageRepository.Add(image);
                }
            }


            if (model.AmentyIds != null && model.AmentyIds.Any())
            {

                var existingAmenities = (await _unitOfWork.RoomAmentyListRepository.Get(filter: ra => ra.RoomId == roomId)).ToList();
                foreach (var amenity in existingAmenities)
                {
                    await _unitOfWork.RoomAmentyListRepository.Remove(amenity);
                }


                foreach (var amentyId in model.AmentyIds)
                {
                    var roomAmentyList = new Data.Entities.RoomAmentiesList
                    {
                        RoomAmentyId = amentyId,
                        RoomId = roomId
                    };
                    await _unitOfWork.RoomAmentyListRepository.Add(roomAmentyList);
                }
            }

            await _unitOfWork.RoomRepository.Update(room);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> InactiveRoom(string roomId, string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var landlord = (await _unitOfWork.LandlordRepository.Get(filter: l => l.UserId == userId))
                           .FirstOrDefault();

            if (landlord == null)
            {
                throw new UnauthorizedAccessException("User is not a landlord.");
            }

            var room = await _unitOfWork.RoomRepository.GetByIDAsync(roomId);
            if (room == null)
            {
                throw new Exception("Room not found.");
            }

            if (landlord.LandlordId != room.RoomType.LandlordId)
            {
                throw new UnauthorizedAccessException("You are not authorized to inactive this room. Only the landlord can update their rooms.");

            }

            room.Status = StatusEnums.Inactive.ToString();

            await _unitOfWork.RoomRepository.Update(room);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
