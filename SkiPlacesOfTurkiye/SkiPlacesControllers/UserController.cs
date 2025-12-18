using Business.Abstract;
using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SkiPlacesOfTurkiye.SkiPlacesControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost(nameof(Create))]
        public IActionResult Create(CreateUserDto input)
        {
            userService.Create(input);
            return Ok(input);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string? id)
        {
            var result = userService.GetById(id);
            return Ok(result);
        }

        [HttpGet(nameof(GetByEmail))]
        public IActionResult GetByEmail(string? email)
        {
            var result = userService.GetByEmail(email);
            return Ok(result);
        }


        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UpdateUserDto input)
        {
            try
            {
                var updatedUser = userService.Update(id, input);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("assign-role")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AssignRole([FromBody] AssignRoleDto request)
        {
            try
            {
                userService.AssignRole(request);
                return Ok(new { Message = "Kullanıcı rolü başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
