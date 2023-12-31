using Core.App.Models;
using Core.App.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace Integration.Repository
{
    public class GoogleCloudStorageService : ICloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly FirestoreDb _firestoreDb;

        public GoogleCloudStorageService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var jsonKeyFilePath = "D:/Downloads/aprobadasperoaquecosto-6aebddcaa7e2.json";
            var projectId = "aprobadasperoaquecosto";

            try {
                _storageClient = StorageClient.Create(GoogleCredential.FromFile(jsonKeyFilePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar StorageClient: {ex.Message}");
                throw;
            }

            try { 
                var builder = new FirestoreClientBuilder
                {
                    JsonCredentials = File.ReadAllText(jsonKeyFilePath)
                };

                _firestoreDb = FirestoreDb.Create(projectId, builder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar Firestore: {ex.Message}");
                throw;
            }
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

        public void UploadFile(string bucketName, string fileName, string contentType, Stream stream)
        {
            try
            {
                _storageClient.UploadObject(bucketName, fileName, contentType, stream);

                Console.WriteLine($"Archivo {fileName} subido exitosamente al bucket {bucketName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo al bucket: {ex.Message}");
            }
        }

        public IEnumerable<string> ListFiles(string bucketName)
        {
            var objects = _storageClient.ListObjects(bucketName);

            foreach (var storageObject in objects)
            {
                yield return storageObject.Name;
            }
        }

        public string GetUrlResource(string bucketName, string fileName)
        {
            return $"https://storage.googleapis.com/{bucketName}/{fileName}";
        }

        public async Task<IEnumerable<VideoMetadata>> ListVideoMetadataAsync()
        {
            var collectionName = "videos";
            List<VideoMetadata> res = new List<VideoMetadata>();
            var videosCollection = _firestoreDb.Collection(collectionName).ListDocumentsAsync();
            await foreach (var videoDoc in videosCollection)
            {
                var snapshot = videoDoc.GetSnapshotAsync().Result;
                var fields = snapshot.ToDictionary();

                string _nombre = fields["nombre"].ToString();
                string _MiniaturaUrl = fields["miniatura_url"].ToString();
                string _VideoUrl = fields["video_url"].ToString();
                var _Etiquetas = new List<string> { "Prueba 1", "Prueba 2" };

                var videoMetadata = new VideoMetadata {
                    Nombre = _nombre,
                    MiniaturaUrl = _MiniaturaUrl,
                    Etiquetas = _Etiquetas,
                    VideoUrl = _VideoUrl
                };

                res.Add(videoMetadata);
            }
            return res;
        }
    }
}
