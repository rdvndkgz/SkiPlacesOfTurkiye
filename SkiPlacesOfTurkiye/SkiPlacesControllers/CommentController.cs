using Business.Abstract;
using Business.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SkiPlacesOfTurkiye.SkiPlacesControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("add")]
        [Authorize] // Token zorunlu! (Role 0 veya 1 fark etmez)
        public IActionResult Add([FromBody] AddCommentDto request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                    return Unauthorized("Kullanıcı doğrulanamadı.");

                Guid userId = Guid.Parse(userIdString);

                _commentService.AddComment(request, userId);

                return Ok(new { Message = "Yorum başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("getbyskiarea/{skiAreaId}")]
        public IActionResult GetBySkiArea(Guid skiAreaId)
        {
            var comments = _commentService.GetCommentsBySkiArea(skiAreaId);
            return Ok(comments);
        }
    }
}
