using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.AmentitiesModel
{
    public class RoomAmentyRequestCreateModel
    {
            //[Required]
            //public string RoomAmentyId { get; set; } = null!;

            [MaxLength(100)]
            public string? Name { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Compensation must be a positive value.")]
            public decimal? Compensation { get; set; }

            //public string? LandlordId { get; set; }
    }
}
