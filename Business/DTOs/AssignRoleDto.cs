using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; } 
        public Role NewRole { get; set; } 
    }
}
