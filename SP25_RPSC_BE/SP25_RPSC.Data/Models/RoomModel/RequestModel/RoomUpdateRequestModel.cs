using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RequestModel
{
    public class RoomUpdateRequestModel
    {
        public string? RoomNumber { get; set; }
        //public string? Title { get; set; }
        //public DateTime? AvailableDateToRent { get; set; }
        //public string? Description { get; set; }
        public decimal? Price { get; set; }
        //public string? RoomTypeId { get; set; }
        public List<string>? AmentyIds { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
