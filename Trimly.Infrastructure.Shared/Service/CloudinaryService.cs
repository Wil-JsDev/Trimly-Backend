using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Trimly.Core.Application.Interfaces.Service;
using Trimly.Core.Domain.Settings;

namespace Trimly.Infrastructure.Shared.Service
{
    public class CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings) : ICloudinaryService
    {
        private CloudinarySettings _cloudinarySettings { get; } = cloudinarySettings.Value;

        public async Task<string> UploadImageCloudinaryAsync(Stream fileStream, string imageName, CancellationToken cancellationToken)
        {
            Cloudinary cloudinary = new Cloudinary(_cloudinarySettings.CloudinaryUrl);
            ImageUploadParams image = new()
            {
                File = new FileDescription(imageName,fileStream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };

            var uploadResult = await cloudinary.UploadAsync(image,cancellationToken);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
