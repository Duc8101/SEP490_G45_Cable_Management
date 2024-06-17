using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.UserDTO
{
    public class TokenDTO
    {
        public string Access_Token { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
