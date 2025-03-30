using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Request
{
    public class CreateRoommatePostReq
    {
        [Required(ErrorMessage = "Post title cannot be empty")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 100 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description cannot be empty")]
        [StringLength(2000, MinimumLength = 30, ErrorMessage = "Description must be between 30 and 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price cannot be empty")]
        [Range(0.01, 100000000, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Rental room ID cannot be empty")]
        public string? RentalRoomId { get; set; }
    }
}
