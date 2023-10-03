using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.UserDTO
{
    public class UserListDTO : UserCreateDTO
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
