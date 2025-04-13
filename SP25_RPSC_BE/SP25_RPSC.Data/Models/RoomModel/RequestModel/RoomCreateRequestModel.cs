using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RequestModel
{
    public class RoomCreateRequestModel
    {

        public string RoomNumber { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string roomtypeId { get; set; }

        public decimal price { get; set; }

        public DateTime? AvailableDateToRent { get; set; }

        public string? Location { get; set; }

        public required List<IFormFile> Images { get; set; }

        public List<string> AmentyId { get; set; }

        //public ICollection<RoomAmentyCreateModel> roomAmentyCreateModels { get; set; }

    }

    //public class RoomAmentyCreateModel
    //{
    //    public string AmentyId { get; set; }
    //}
}
