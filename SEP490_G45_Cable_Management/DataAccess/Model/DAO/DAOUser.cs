using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using DataAccess.Model.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOUser : BaseDAO
    {
        public async Task<User?> getUser(LoginDTO DTO)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.UserName == DTO.UserName);
            // if username or password in correct
            if (user == null || string.Compare(user.Password, UserUtil.HashPassword(DTO.Password), false) != 0)
            {
                return null;
            }
            return user;
        }
    }
}
