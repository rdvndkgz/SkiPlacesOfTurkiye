using Business.Abstract;
using Business.DTOs;
using DataAccess.Abstract;
using Entities.Entity;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private IRepository<User> userRepository;
        private IUnitOfWork unitOfWork;
        private HttpContext httpContext;

        public UserService(IRepository<User> userRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public void Create(CreateUserDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (string.IsNullOrWhiteSpace(input.Username) ||
                string.IsNullOrWhiteSpace(input.Email) ||
                string.IsNullOrWhiteSpace(input.Password))
                throw new ArgumentException("Username, Email ve Password gereklidir.");

            byte[] passwordHash;
            using (var sha256 = SHA256.Create())
            {
                passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input.Password));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = input.Username,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                Password = passwordHash
            };

            userRepository.Add(user);
            unitOfWork.Save();
        }


        public void Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
                throw new InvalidDataException(id);

            var deletedUser = userRepository.Get(user => user.Id.Equals(userId));

            userRepository.Delete(deletedUser);
            unitOfWork.Save();
        }

        public UserDto GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));

            var user = userRepository.Get(u => u.Email == email);

            if (user == null)
                throw new NullReferenceException(nameof(User));

            return new UserDto
            {
                Username = user.Username,
                Email = user.Email
            };
        }

        public UserDto GetById(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
                throw new InvalidDataException("Id is invalid.");

            var user = userRepository.Get(u => u.Id == userId);

            if (user == null)
                throw new NullReferenceException(nameof(User));

            return new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email
            };
        }

        public UserDto Update(string userId, UpdateUserDto input)
        {
            if (!Guid.TryParse(userId, out Guid guidId))
            {
                throw new Exception("Geçersiz ID formatı.");
            }

            var user = userRepository.Get(x => x.Id == guidId);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (!string.IsNullOrEmpty(input.Email) && user.Email != input.Email)
            {
                var existingUser = userRepository.Get(u => u.Email == input.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    throw new Exception("Bu e-posta adresi zaten kullanımda.");
                }
                user.Email = input.Email;
            }

            if (!string.IsNullOrEmpty(input.Username)) user.Username = input.Username;
            if (!string.IsNullOrEmpty(input.FirstName)) user.FirstName = input.FirstName;
            if (!string.IsNullOrEmpty(input.LastName)) user.LastName = input.LastName;

            userRepository.Update(user);
            unitOfWork.Save();

            return new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public void AssignRole(AssignRoleDto assignRoleDto)
        {
            var user = userRepository.Get(u => u.Id == assignRoleDto.UserId);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı!");
            }

            // 2. Rolünü güncelle
            // (İstersen burada sadece 0 ve 1 gelebilir diye kontrol de koyabilirsin)
            user.Role = assignRoleDto.NewRole;

            // 3. Veritabanına kaydet
            userRepository.Update(user); // Repository'inde Update metodu varsa
            unitOfWork.Save();
        }
    }
}
