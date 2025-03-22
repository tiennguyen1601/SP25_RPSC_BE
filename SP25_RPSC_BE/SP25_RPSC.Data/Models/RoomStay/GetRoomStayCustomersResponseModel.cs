using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomStay
{
    public class GetRoomStayCustomersResponseModel
    {
        public RoomStayDetailsDto? RoomStay { get; set; } 
        public List<RoomStayCustomerDto> RoomStayCustomers { get; set; } = new List<RoomStayCustomerDto>();
        public int TotalCustomers { get; set; }
    }

    public class RoomStayCustomerDto
    {
        public string RoomStayCustomerId { get; set; } = null!;
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
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

        public List<RoomImageDto> RoomImages { get; set; } = new();
        public List<RoomAmentiesListDto> RoomAmentiesLists { get; set; } = new();
        public List<RoomPriceDto> RoomPrices { get; set; } = new();

        public RoomTypeDto? RoomType { get; set; }
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
    public class RoomPriceDto
    {
        public string PriceId { get; set; } = null!;
        public decimal? Price { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }



    public class RoomTypeDto
    {
        public string RoomTypeId { get; set; } = null!;
        public string? RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }
        public int? Area { get; set; }
        public decimal? Square { get; set; }
        public string? Description { get; set; }
        public int? MaxOccupancy { get; set; }
        public string? Status { get; set; }

        public List<RoomServiceDto> RoomServices { get; set; } = new();
    }

    public class RoomServiceDto
    {
        public string ServiceId { get; set; } = null!;
        public string? ServiceName { get; set; }
        public decimal? Cost { get; set; }
    }

}
