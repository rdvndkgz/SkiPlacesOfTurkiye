using Entities.Enum;

namespace Entities.Entity
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Password { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
