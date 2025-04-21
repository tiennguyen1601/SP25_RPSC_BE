using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

public class FeedBackRoommateRequestModel
{
    [Required(ErrorMessage = "RevieweeId is required")]
    public string RevieweeId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [MaxLength(3, ErrorMessage = "Maximum 3 images are allowed")]
    public required List<IFormFile> Images { get; set; } = new List<IFormFile>();

    // Custom validation for image files
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Check if Images is not null before validation
        if (Images != null)
        {
            // Validate file count
            if (Images.Count > 3)
            {
                yield return new ValidationResult("Maximum 3 images are allowed", new[] { nameof(Images) });
            }

            // Validate each file
            foreach (var image in Images)
            {
                // Validate file size (max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    yield return new ValidationResult($"Image {image.FileName} exceeds maximum size of 5MB", new[] { nameof(Images) });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
                if (!allowedTypes.Contains(image.ContentType.ToLower()))
                {
                    yield return new ValidationResult($"File {image.FileName} is not a valid image type. Allowed types: JPEG, JPG, PNG, GIF", new[] { nameof(Images) });
                }
            }
        }
    }
}
