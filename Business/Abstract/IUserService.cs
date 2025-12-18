using Business.DTOs;

namespace Business.Abstract
{
    public interface IUserService
    {
        void Create(CreateUserDto input);
        UserDto GetById(string id);
        UserDto GetByEmail(string email);
        UserDto Update(string userId, UpdateUserDto input);
        void Delete(string id);
        void AssignRole(AssignRoleDto assignRoleDto);
    }
}
