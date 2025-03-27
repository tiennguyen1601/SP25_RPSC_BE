using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerResponse
{
    public class GetContractsByLandlordResponseModel
    {
        public List<CustomerContractResponse> Contracts { get; set; }
        public int TotalContracts { get; set; }
    }

    public class CustomerContractResponse
    {
        public string ContractId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }

        public RoomResponse Room { get; set; }
        public UserResponse Customer { get; set; }

        public DateTime? TimeToUpContract { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
    }


    public class RoomResponse
    {
        // Thông tin RoomType
        public string RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }
        public int? MaxOccupancy { get; set; }
        public string? RoomTypeStatus { get; set; }

        // Thông tin Room
        public string RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }

    public class UserResponse
    {
        // Thông tin User
        public string UserId { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }

        // Thông tin Customer
        public string CustomerId { get; set; }
        public string? Preferences { get; set; }
        public string? LifeStyle { get; set; }
        public string? BudgetRange { get; set; }
        public string? PreferredLocation { get; set; }
        public string? Requirement { get; set; }
        public string? CustomerType { get; set; }
    }
}
