﻿using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.PackageModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomTypeModel.Request
{
    public class RoomTypeCreateRequestModel

    {
        [JsonIgnore]
        public Guid RoomTypeCreateRequestModelId { get; set; } = Guid.NewGuid();

        public string RoomTypeName { get; set; }

        public decimal Deposite { get; set; }

        public int Area { get; set; }

        public decimal Square { get; set; }

        public string Description { get; set; }

        public int MaxOccupancy { get; set; }
       
        public Location location { get; set; }

        public ICollection<RoomServiceRequestCreate> ListRoomServices { get; set; }
    }

    public class Location
    {
        public double Long { get; set; }

        public double Lat { get; set; }
        public string? HouseNumber { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }

    }
    public class RoomServiceRequestCreate
    {

        public string RoomServiceName { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public string status = StatusEnums.Active.ToString();

        public RoomServicePriceRequest Price { get; set; }
    }
     
    public class RoomServicePriceRequest
    {

        [JsonIgnore]
        public Guid RoomServicePriceId { get; set; } = Guid.NewGuid();

        public decimal Price { get; set; }

    }
}
