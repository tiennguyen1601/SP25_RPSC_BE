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
        Task<string> UploadImageAsync(IFormFile file);
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

        public async Task<string> UploadImageAsync(IFormFile file)
        {
          

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
