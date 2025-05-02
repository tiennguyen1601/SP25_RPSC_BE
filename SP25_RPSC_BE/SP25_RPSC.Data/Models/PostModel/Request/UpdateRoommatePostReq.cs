using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Request
{
    public class UpdateRoommatePostReq
    {
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 100 characters")]
        public string? Title { get; set; }

        [StringLength(2000, MinimumLength = 30, ErrorMessage = "Description must be between 30 and 2000 characters")]
        public string? Description { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

    }
}
