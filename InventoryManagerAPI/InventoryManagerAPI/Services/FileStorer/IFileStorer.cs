using CloudinaryDotNet.Actions;

namespace InventoryManagerAPI.FileStorer;

public interface IFileStorer
{
    public Task<string> UploadImageAsync(ImageUploadParams imageParams);
    public Task<string> UploadDocumentAsync(RawUploadParams uploadParams);
}
