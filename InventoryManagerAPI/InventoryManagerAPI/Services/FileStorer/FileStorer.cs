using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace InventoryManagerAPI.FileStorer;

public sealed class FileStorer : IFileStorer
{
    private readonly Cloudinary _cloudinary;

    public FileStorer(Cloudinary cloudinary) =>
        _cloudinary = cloudinary;
    

    public async Task<string> UploadImageAsync(ImageUploadParams imageParams)
    {
        var result = await _cloudinary.UploadAsync(imageParams);

        return result.Url.AbsoluteUri;
    }

    public async Task<string> UploadDocumentAsync(RawUploadParams uploadParams)
    {
        var result = await _cloudinary.UploadAsync(uploadParams);

        return result.Url.AbsoluteUri;
    }
}
