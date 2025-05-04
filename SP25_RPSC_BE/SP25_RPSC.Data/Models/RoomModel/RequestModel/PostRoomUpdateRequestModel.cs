using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.RoomModel.RequestModel
{
   public class PostRoomUpdateRequestModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int? DateExist { get; set; }

        [Required]
        public DateTime AvailableDateToRent { get; set; }
    }
}
