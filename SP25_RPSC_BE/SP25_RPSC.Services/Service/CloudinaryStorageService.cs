using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SP25_RPSC.Services.Service
{
    public interface ICloudinaryStorageService
    {
        Task<List<string>> UploadImageAsync(List<IFormFile> file);
    }


    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;
        public CloudinaryStorageService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            var account = new Account(
          _config["CloudinarySettings:CloudName"],
           _config["CloudinarySettings:ApiKey"],
           _config["CloudinarySettings:ApiSecret"]
       );
             _cloudinary = new Cloudinary(account);
        }


        public async Task<List<string>> UploadImageAsync(List<IFormFile> files)
        {
            var uploadUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("One of the files is invalid.");
                }

                try
                {
                    await using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    uploadUrls.Add(uploadResult.SecureUrl.AbsoluteUri); // Lấy URL ảnh đã tải lên
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upload image {file.FileName}: " + ex.Message);
                }
            }
            return uploadUrls;
        }

        public async Task<DeletionResult> DeleteFile(string publicId)
        {
            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Video  // Specify that the resource type is video
            };
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
