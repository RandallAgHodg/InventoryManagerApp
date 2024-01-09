using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace InventoryManagerAPI.Extensions;

public static class FileExtensions
{
    public static ImageUploadParams ToImageParams(this IFormFile file) =>
        new()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream())
        };

    public static RawUploadParams ToRawUploadParams(this byte[] data, Guid reportId) =>
        new()
        {
            File = new FileDescription($"report - {reportId}", new MemoryStream(data))
        };
}
