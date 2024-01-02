using Core.App.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Pre.MultiPlatform.Blazor.Integration
{
    public class VideoStreamingApiHandler
    {
        private string _RequestUrl;
        private readonly HttpClient _httpClient;

        public VideoStreamingApiHandler(string url, HttpClient httpClient)
        {
            _RequestUrl = url;
            _httpClient = httpClient;
        }

        public async Task<List<VideoMetadata>> GetAllVideos()
        {
            List<VideoMetadata> res = new List<VideoMetadata>();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_RequestUrl + "api/getAllVideos"),
                Method = HttpMethod.Get
            };

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<List<VideoMetadata>>(jsonResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al procesar la respuesta: {ex.Message}");
            }

            return res;
        }

        public async Task<List<VideoMetadata>> SearchAsync(string query)
        {
            List<VideoMetadata> res = new List<VideoMetadata>();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_RequestUrl + $"api/search/{query}"),
                Method = HttpMethod.Get
            };

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<List<VideoMetadata>>(jsonResponse);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return res;
        }

        public async Task UploadFileAsync(string filePath)
        {
            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "file",
                            FileName = Path.GetFileName(filePath)
                        };

                        formData.Add(fileContent);

                        var response = await httpClient.PostAsync(_RequestUrl + "api/upload", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Archivo subido exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine($"Error al subir el archivo. Código de estado: {response.StatusCode}");
                        }
                    }
                }
            }
        }
    }
}
