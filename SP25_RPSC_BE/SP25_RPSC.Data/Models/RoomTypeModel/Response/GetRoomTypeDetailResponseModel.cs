using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.RoomStay;

namespace SP25_RPSC.Data.Models.RoomTypeModel.Response
{
    public class GetRoomTypeDetailResponseModel
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? Deposite { get; set; }
        public int? Area { get; set; }
        public decimal? Square { get; set; }
        public string Description { get; set; }
        public int? MaxOccupancy { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public AddressDto Address { get; set; }  
        public List<RoomServiceRoomTypeDto> RoomServices { get; set; }  
    }

    public class AddressDto
    {
        public string AddressId { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }
    }
    public class RoomServiceRoomTypeDto
    {
        public string RoomServiceId { get; set; }
        public string RoomServiceName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal? Price { get; set; }  
    }




}
