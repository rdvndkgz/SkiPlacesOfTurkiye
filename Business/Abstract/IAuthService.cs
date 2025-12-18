using Business.DTOs;
using Entities.Entity;

namespace Business.Abstract
{
    public interface IAuthService
    {
        LoginResponseDto Register(RegisterRequest request);
        LoginResponseDto Login(LoginDto dto);
    }
}
