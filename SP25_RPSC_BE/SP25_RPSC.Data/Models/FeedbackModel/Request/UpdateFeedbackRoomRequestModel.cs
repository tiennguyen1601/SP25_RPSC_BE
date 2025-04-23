using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Request
{
    public class UpdateFeedbackRoomRequestModel
    {
        [Required(ErrorMessage = "FeedbackId is required")]
        public string FeedbackId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(3, ErrorMessage = "Maximum 3 images are allowed")]
        public required List<IFormFile> Images { get; set; } = new List<IFormFile>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Images != null)
            {
                if (Images.Count > 3)
                {
                    yield return new ValidationResult("Maximum 3 images are allowed", new[] { nameof(Images) });
                }

                foreach (var image in Images)
                {
                    if (image.Length > 5 * 1024 * 1024)
                    {
                        yield return new ValidationResult($"Image {image.FileName} exceeds maximum size of 5MB", new[] { nameof(Images) });
                    }

                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
                    if (!allowedTypes.Contains(image.ContentType.ToLower()))
                    {
                        yield return new ValidationResult($"File {image.FileName} is not a valid image type. Allowed types: JPEG, JPG, PNG, GIF", new[] { nameof(Images) });
                    }
                }
            }
        }
    }
}
