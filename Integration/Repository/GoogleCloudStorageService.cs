using Core.App.Models;
using Core.App.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using static System.Net.Mime.MediaTypeNames;

namespace Integration.Repository
{
    public class GoogleCloudStorageService : ICloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly FirestoreDb _firestoreDb;

        public GoogleCloudStorageService()
        {
            var jsonKeyFilePath = "./credential.json";
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

        public bool FileExistsInBucket(string bucketName, string fileName)
        {
            try
            {
                var objects = _storageClient.ListObjects(bucketName);

                return objects.Any(storageObject => storageObject.Name == fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar la existencia del archivo en el bucket: {ex.Message}");
                throw;
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
                List<object> _temp = (List<object>)fields["etiquetas"];
                List<string> _temp2 = new List<string>();
                foreach (var obj in _temp)
                {
                    _temp2.Add(obj.ToString());
                }
                var videoMetadata = new VideoMetadata {
                    Nombre = fields["nombre"].ToString(),
                    MiniaturaUrl = fields["miniatura_url"].ToString(),
                    Etiquetas = _temp2,
                    VideoUrl = fields["video_url"].ToString()
                };
                res.Add(videoMetadata);
            }
            return res;
        }

        public async Task<IEnumerable<string>> GetAllLabels()
        {
            try
            {
                List<string> videos = new List<string>();
                var etiquetasCollection = _firestoreDb.Collection("etiquetas");
                var query = etiquetasCollection.ListDocumentsAsync();
                await foreach (var item in query)
                {
                    videos.Add(item.Id.ToString());
                }
                return videos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener etiquetas: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetLabels(int maxCount)
        {
            try
            {
                List<string> videos = new List<string>();
                var etiquetasCollection = _firestoreDb.Collection("etiquetas");
                Query query = etiquetasCollection.Limit(maxCount);
                QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                {
                    videos.Add(documentSnapshot.Id.ToString());
                }
                return videos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener etiquetas: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetVideosByLabelAsync(string label)
        {
            try
            {
                List<string> videos = new List<string>();
                var etiquetasCollection = _firestoreDb.Collection("etiquetas");
                var query = etiquetasCollection.Document(label);
                if(query != null)
                {
                    var snapshot = await query.GetSnapshotAsync();
                    if (snapshot.Exists == false) return videos;
                    var fields = snapshot.ToDictionary();
                    foreach(var field  in fields)
                    {
                        videos.Add(field.Key.ToString());
                    }
                }
                return videos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar etiqueta: {ex.Message}");
                throw;
            }
        }

        public async Task<VideoMetadata?> GetVideoMetadataByNameAsync(string names)
        {
            try
            {
                var etiquetasCollection = _firestoreDb.Collection("videos");
                var query = etiquetasCollection.Document(names);
                if (query != null)
                {
                    var snapshot = await query.GetSnapshotAsync();
                    if (snapshot.Exists == false) return null;
                    var fields = snapshot.ToDictionary();
                    List<object> _temp = (List<object>)fields["etiquetas"];
                    List<string> _temp2 = new List<string>();
                    foreach (var obj in _temp)
                    {
                        _temp2.Add(obj.ToString());
                    }
                    var videoMetadata = new VideoMetadata
                    {
                        Nombre = fields["nombre"].ToString(),
                        MiniaturaUrl = fields["miniatura_url"].ToString(),
                        Etiquetas = _temp2,
                        VideoUrl = fields["video_url"].ToString()
                    };
                    return videoMetadata;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar video: {ex.Message}");
                throw;
            }
        }
    }
}