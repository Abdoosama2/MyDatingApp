using CloudinaryDotNet.Actions;

namespace DatingApp.Application.Services.Interfaces
{
    public interface IPhotoService
    {

        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);


        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
