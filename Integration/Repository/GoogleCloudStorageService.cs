using Core.App.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace Integration.Repository
{
    public class GoogleCloudStorageService : ICloudStorageService
    {
        private readonly StorageClient _storageClient;

        public GoogleCloudStorageService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var jsonKeyFilePath = configuration["GoogleCloud:JsonKeyFilePath"];

            _storageClient = StorageClient.Create(GoogleCredential.FromFile(jsonKeyFilePath));
        }

        public void UploadFile(string bucketName, string filePath, string contentType)
        {
            try
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    _storageClient.UploadObject(bucketName, Path.GetFileName(filePath), contentType, fileStream);
                }

                Console.WriteLine($"Archivo {Path.GetFileName(filePath)} subido exitosamente al bucket {bucketName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo al bucket: {ex.Message}");
            }
        }
    }
}
