using Core.App.Models;
using Core.App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CloudStorage : ControllerBase
    {
        private readonly ILogger<CloudStorage> _logger;
        private readonly ICloudStorageService _cloudStorageService;

        public CloudStorage(ILogger<CloudStorage> logger, ICloudStorageService cloudStorageService)
        {
            _logger = logger;
            _cloudStorageService = cloudStorageService;
        }

        [HttpPost("uploadVideo")]
        public IActionResult UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            var bucketName = "cloud-videos";
            var contentType = file.ContentType;
            var fileName = file.FileName;

            using (var stream = file.OpenReadStream())
            {
                _cloudStorageService.UploadFile(bucketName, fileName, contentType, stream);
            }

            return Ok($"Video {fileName} subido exitosamente al bucket {bucketName}");
        }

        [HttpGet("listVideoMetadata")]
        public IActionResult ListVideoMetadata()
        {
            try
            {
                var videos = _cloudStorageService.ListVideoMetadataAsync().Result;
                return Ok(videos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar la solicitud: {ex.Message}");

                return Ok(new { Success = false, ErrorMessage = "Error al procesar la solicitud." });
            }
        }

        [HttpGet("search")]
        public IActionResult Search(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return BadRequest("El texto no puede estar vacío.");
                }

                var words = text.Split(' ');

                HashSet<string> nombre_videos = new HashSet<string>();
                foreach (var word in words)
                {
                    var queryvideos = _cloudStorageService.GetVideosByLabelAsync(word).Result;
                    foreach (var video in queryvideos) {
                        nombre_videos.Add(video);
                    }
                }

                List<VideoMetadata> video_metadata = new List<VideoMetadata>();
                foreach(var video in nombre_videos)
                {
                    var temp = _cloudStorageService.GetVideoMetadataByNameAsync(video).Result;
                    if(temp != null) { video_metadata.Add(temp); }
                }

                return Ok(video_metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar la solicitud: {ex.Message}");

                return Ok(new { Success = false, ErrorMessage = "Error al procesar la solicitud." });
            }
        }
    }
}
