using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _client;

        public CloudinaryStorageService(IConfiguration config)
        {
            var acc = new Account(
              config["Cloudinary:CloudName"],
              config["Cloudinary:ApiKey"],
              config["Cloudinary:ApiSecret"]
            );
            _client = new Cloudinary(acc);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "orchids"  
            };
            var result = await _client.UploadAsync(uploadParams);
            if (result.Error != null)
                throw new ApplicationException(result.Error.Message);
            return result.SecureUrl.ToString();
        }
    }
}
