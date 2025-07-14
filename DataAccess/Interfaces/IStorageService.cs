using Microsoft.AspNetCore.Http;

namespace DataAccess.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}