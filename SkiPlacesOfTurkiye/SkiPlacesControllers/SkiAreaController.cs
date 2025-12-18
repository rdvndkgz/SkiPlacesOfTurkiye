using Business.Abstract;
using Business.DTOs.SkiAreaDtos;
using Business.Services;
using Entities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkiPlacesOfTurkiye.SkiPlacesControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkiAreaController : ControllerBase
    {
        private ISkiAreaService skiAreaService;

        public SkiAreaController(ISkiAreaService skiAreaService)
        {
            this.skiAreaService = skiAreaService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost(nameof(Create))]
        public IActionResult Create(CreateSkiAreaDto input)
        {
            skiAreaService.Create(input);
            return Ok(input);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete(nameof(Delete))]
        public IActionResult Delete(string id)
        {
            skiAreaService.Delete(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("upload-photo")]
        [Consumes("multipart/form-data")]
        public IActionResult UploadPhoto([FromForm] SkiAreaPhotoUploadDto dto)
        {
            skiAreaService.UploadPhoto(dto);
            return Ok("Fotoğraf başarıyla yüklendi");
        }

        [Authorize]
        [HttpGet("by-name/{name}")]
        public IActionResult GetByName(string name)
        {
            var result = skiAreaService.GetBySkiAreaName(name);

            if (result == null)
            {
                return NotFound(new { message = "Bu isimde bir kayak merkezi bulunamadı." });
            }

            return Ok(result);
        }
    }
}
