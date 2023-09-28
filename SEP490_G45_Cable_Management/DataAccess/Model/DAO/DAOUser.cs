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

        public async Task<int> CreateUser(User user)
        {
            await context.Users.AddAsync(user);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> isExist(string username, string email)
        {
            return await context.Users.AnyAsync(u => (u.UserName == username || u.Email == email) && u.IsDeleted == false);
        }

        public async Task<int> UpdateUser(User user)
        {
            context.Users.Update(user);
            return await context.SaveChangesAsync();
        }

        public async Task<User?> getUser(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
