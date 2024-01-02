using Core.App.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace Pre.MultiPlatform.Integration
{
    public class VideoStreamingApiHandler
    {
        private string _RequestUrl;
        HttpClient _httpClient;

        public VideoStreamingApiHandler(string url, HttpClient httpClient)
        {
            _RequestUrl = url;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(url);
        }

        public async Task<List<VideoMetadata>> GetAllVideos()
        {
            List<VideoMetadata> res = new List<VideoMetadata>();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_RequestUrl + "api/getAllVideos"),
                Method = HttpMethod.Get
            };

            using (var response = _httpClient.SendAsync(request).Result)
            {
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<List<VideoMetadata>>(jsonResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Excepción al procesar la respuesta: {ex.Message}");
                }
            }
            return res;
        }

    }
}
