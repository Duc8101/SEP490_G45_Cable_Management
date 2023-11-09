using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.UserDTO
{
    public class LoginDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
