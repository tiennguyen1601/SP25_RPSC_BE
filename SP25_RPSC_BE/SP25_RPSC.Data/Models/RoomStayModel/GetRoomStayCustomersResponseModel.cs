using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Models.RoomStay
{
    public class GetRoomStayCustomersResponseModel
    {
        public RoomStayDetailsDto? RoomStay { get; set; }
        public List<RoomStayCustomerDto> RoomStayCustomers { get; set; } = new();
        public int TotalCustomers { get; set; }
    }

    public class RoomStayCustomerDto
    {
        public string RoomStayCustomerId { get; set; } = null!;
        public string? Type { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? Preferences { get; set; }
        public string? LifeStyle { get; set; }
        public string? BudgetRange { get; set; }
        public string? PreferredLocation { get; set; }
        public string? Requirement { get; set; }
        public string? Address { get; set; }
        public string? UserId { get; set; }
    }

    public class RoomStayDetailsDto
    {
        public string RoomStayId { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RoomDto? Room { get; set; }
    }

    public class RoomDto
    {
        public string RoomId { get; set; } = null!;
        public string? RoomNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string RoomTypeId { get; set; } = null!;
        public string? RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }
        public int? Area { get; set; }
        public decimal? Square { get; set; }
        public string? RoomTypeDescription { get; set; }
        public int? MaxOccupancy { get; set; }
        public decimal? Price { get; set; }
        public List<PostRoomDto> PostRooms { get; set; } = new();
        public List<RoomImageDto> RoomImages { get; set; } = new();
        public List<RoomAmentiesListDto> RoomAmentiesLists { get; set; } = new();
        public List<RoomServiceDto> RoomServices { get; set; } = new();
    }
    public class PostRoomDto
    {
        public string PostRoomId { get; set; }
        public DateTime? DateUpPost { get; set; }
        public int? DateExist { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class RoomAmentiesListDto
    {
        public string AmenityId { get; set; } = null!;
        public string? Name { get; set; }
    }

    public class RoomImageDto
    {
        public string ImageId { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }

    public class RoomServiceDto
    {
        public string ServiceId { get; set; } = null!;
        public string? ServiceName { get; set; }
        public decimal? Cost { get; set; }
    }
}
