namespace BusinessLogicLayer.Services.BlobService
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName, string containerName);
        Task DeleteFileAsync(string blobUrl);
    }
}
