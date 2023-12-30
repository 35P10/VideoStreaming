﻿namespace Core.App.Services
{
    public interface ICloudStorageService
    {
        void UploadFile(string bucketName, string filePath, string contentType);
        IEnumerable<string> ListFiles(string bucketName);
        string GetUrlResource(string bucketName, string fileName);
    }
}
