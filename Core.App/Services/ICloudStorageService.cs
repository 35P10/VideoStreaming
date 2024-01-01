using Core.App.Models;

namespace Core.App.Services
{
    public interface ICloudStorageService
    {
        void UploadFile(string bucketName, string filePath, string contentType);
        void UploadFile(string bucketName, string fileName, string contentType, Stream stream);
        IEnumerable<string> ListFiles(string bucketName);
        bool FileExistsInBucket(string filename, string bucketName);
        string GetUrlResource(string bucketName, string fileName);
        Task<IEnumerable<VideoMetadata>> ListVideoMetadataAsync();
        Task<List<string>> GetVideosByLabelAsync(string label);
        Task<VideoMetadata?> GetVideoMetadataByNameAsync(string names);
    }
}
