using Microsoft.AspNetCore.Http;

namespace Business.DTOs.SkiAreaDtos
{
    public class SkiAreaPhotoUploadDto
    {
        public string SkiAreaName { get; set; }
        public IFormFile File { get; set; }
    }
}
