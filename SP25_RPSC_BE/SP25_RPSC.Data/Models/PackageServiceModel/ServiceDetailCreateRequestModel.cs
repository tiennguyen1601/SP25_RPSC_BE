using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.PackageModel;

namespace SP25_RPSC.Data.Models.PackageServiceModel
{
    public class ServiceDetailCreateRequestModel
    {
        [Required(ErrorMessage = "PackageId is not empty")]
        public string PackageId { get; set; }

        [Required(ErrorMessage = "Name is not empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Duration is not empty")]
        public string? Duration { get; set; }
        [Required(ErrorMessage = "Description is not empty")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Price phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

    }


    public class PricePackageRequestModel
    {
        [Required(ErrorMessage = "Price không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Price phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "ApplicableDate không được để trống")]
        public DateTime ApplicableDate { get; set; }
    }


}
