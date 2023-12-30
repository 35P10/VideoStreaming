namespace Core.App.Services
{
    public interface ICloudStorageService
    {
        void UploadFile(string bucketName, string filePath, string contentType);
    }
}
