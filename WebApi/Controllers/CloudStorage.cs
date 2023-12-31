using Core.App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

            return Ok($"Video {fileName} uploaded successfully to the bucket {bucketName}");
        }

        [HttpGet("listVideos")]
        public IActionResult ListVideos()
        {
            // Assuming 'cloud-videos' is your bucket name for videos
            var bucketName = "cloud-videos";

            var videoList = _cloudStorageService.ListFiles(bucketName);

            return Ok(videoList);
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

                return Ok(new { Success = false, ErrorMessage = "Error al procesar la solicitud. Consulta los registros para obtener más detalles." });
            }
        }
    }
}
