using DataAccess.Const;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using DataAccess.Model.Util;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Operators;
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
            User? user = await context.Users.SingleOrDefaultAsync(u => u.UserName == DTO.UserName && u.IsDeleted == false);
            // if username or password invalid
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
            return await context.Users.AnyAsync(u => u.UserName == username || u.Email == email);
        }

        public async Task<int> UpdateUser(User user)
        {
            context.Users.Update(user);
            return await context.SaveChangesAsync();
        }

        public async Task<User?> getUser(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
        }

        public async Task<List<User>> getList(string? filter, int page)
        {
            if(filter == null || filter.Trim().Length == 0)
            {
                return await context.Users.Where(u => u.IsDeleted == false).Skip(PageSizeConst.MAX_USER_LIST_IN_PAGE * (page - 1)).
                    Take(PageSizeConst.MAX_USER_LIST_IN_PAGE).OrderByDescending(u => u.UpdateAt).ToListAsync();
            }
            return await context.Users.Where(u => u.IsDeleted == false && (u.FirstName.ToLower().Contains(filter.ToLower().Trim()) || 
            u.LastName.ToLower().Contains(filter.ToLower().Trim()) || u.Role.RoleName.Contains(filter.ToLower().Trim()) 
            || u.Email.ToLower().Contains(filter.ToLower().Trim()) || u.UserName.ToLower().Contains(filter.ToLower().Trim())))
                .Skip(PageSizeConst.MAX_USER_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_USER_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            List<User> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                list = await context.Users.Where(u => u.IsDeleted == false).OrderByDescending(u => u.UpdateAt).ToListAsync();
            }
            else
            {
                list = await context.Users.Where(u => u.IsDeleted == false && (u.FirstName.ToLower().Contains(filter.ToLower().Trim()) ||
                u.LastName.ToLower().Contains(filter.ToLower().Trim()) || u.Role.RoleName.Contains(filter.ToLower().Trim())
                || u.Email.ToLower().Contains(filter.ToLower().Trim()) || u.UserName.ToLower().Contains(filter.ToLower().Trim())))
                    .OrderByDescending(u => u.UpdateAt).ToListAsync();
            }
            return list.Count;
        }
    }
}
