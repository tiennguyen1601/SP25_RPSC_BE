using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomStayModel
{
    public class GetRoomStayByCustomerIdResponseModel
    {
        public RoomStayDetailsResponseModel? RoomStay { get; set; }
        public CustomerContractDto? CustomerContract { get; set; }
    }


    public class RoomStayDetailsResponseModel
    {
        public string? LandlordName { get; set; }
        public string? LandlordId { get; set; }
        public string? UserId { get; set; }
        public string? LandlordAvatar { get; set; }
        public string? RoomStayCustomerType { get; set; }

        public string? statusOfMaxRoom  { get; set; }

        public string RoomStayId { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RoomCusDto? Room { get; set; }
    }

    public class CustomerContractDto
    {
        public string ContractId { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Term { get; set; }
        public string? TenantId { get; set; }
        public string? RentalRoomId { get; set; }
        public string? RoomStayId { get; set; }
    }

    public class RoomCusDto
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

        public List<RoomImageCusDto> RoomCusImages { get; set; } = new();
        public List<RoomAmentiesCusListDto> RoomCusAmentiesLists { get; set; } = new();
        public List<RoomServiceCusDto> RoomCusServices { get; set; } = new();
    }

    public class RoomAmentiesCusListDto
    {
        public string? Name { get; set; }
    }

    public class RoomImageCusDto
    {
        public string? ImageUrl { get; set; }
    }

    public class RoomServiceCusDto
    {
        public string ServiceId { get; set; } = null!;
        public string? ServiceName { get; set; }
        public decimal? Cost { get; set; }
    }
}
