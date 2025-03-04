
namespace Trimly.Core.Application.Interfaces.Service
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageCloudinaryAsync(
           Stream fileStream,
           string imageName,
           CancellationToken cancellationToken);
    }
}
