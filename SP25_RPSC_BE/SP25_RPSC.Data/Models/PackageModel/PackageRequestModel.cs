using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SP25_RPSC.Data.Models.PackageModel
{
    public class PackageCreateRequestModel
    {
        [JsonIgnore]
        public Guid PackageId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Type không được để trống")]
        public string Type { get; set; }

        [Required(ErrorMessage = "HighLight không được để trống")]
        public string HighLight { get; set; }

        [MaxLength(50, ErrorMessage = "Size không được quá 50 ký tự")]
        public string? Size { get; set; }
    }

    public class PackageCreateDetailReqestModel
    {
        [JsonIgnore]
        public Guid ServiceDetailId { get; set; } = Guid.NewGuid();
        [JsonIgnore]
        public Guid PackageId { get; set; }
        public string Type { get; set; }
        public int LimitPost { get; set; }
        public PricePackageRequestModel PricePackageModel { get; set; }
    }

    public class PricePackageRequestModel
    {
        [JsonIgnore]
        public Guid PriceId { get; set; } = Guid.NewGuid();

        public decimal Price { get; set; }
        
        public DateTime? ApplicableDate { get; set; }

    }
}
