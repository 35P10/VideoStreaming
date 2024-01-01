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

        [HttpPost]
        [Route("upload")]
        public IActionResult UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            if (!IsVideoFile(file))
            {
                return BadRequest("Formato de archivo invalido. Solo archivos de video son permitidos.");
            }

            var bucketName = "cloud-videos";
            var contentType = file.ContentType;

            var fileName = ProcessVideoFileName(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                _cloudStorageService.UploadFile(bucketName, fileName, contentType, stream);
            }

            return Ok($"Video {fileName} subido exitosamente al bucket {bucketName}");
        }

        private bool IsVideoFile(IFormFile file)
        {
            var allowedVideoExtensions = new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedVideoExtensions.Contains(fileExtension);
        }

        private string ProcessVideoFileName(string originalFileName)
        {
            var processedFileName = Path.GetFileNameWithoutExtension(originalFileName);
            processedFileName = new string(processedFileName
                .Where(c => Char.IsLetterOrDigit(c))
                .ToArray());
            return processedFileName.ToLower();
        }

        [HttpGet]
        [Route("getAllVideos")]
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

        [HttpGet]
        [Route("search/{text}")]
        public IActionResult Search(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return BadRequest("El texto no puede estar vacío.");
                }

                text = new string(text.Where(c => Char.IsLetter(c)).ToArray());
                text = text.ToLower();
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
