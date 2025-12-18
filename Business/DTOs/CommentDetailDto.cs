using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class CommentDetailDto
    {
            public Guid Id { get; set; }
            public string Content { get; set; }
            public string Username { get; set; }
            public DateTime CreatedDate { get; set; }
    }
}
