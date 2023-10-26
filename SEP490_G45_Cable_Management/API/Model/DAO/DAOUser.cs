using DataAccess;
using DataAccess.Const;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOUser : BaseDAO
    {
        public async Task<User?> getUser(LoginDTO DTO)
        {
            User? user = await context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Username == DTO.UserName && u.IsDeleted == false);
            // if username or password invalid
            if (user == null || string.Compare(user.Password, UserUtil.HashPassword(DTO.Password), false) != 0)
            {
                return null;
            }
            return user;
        }

        public void CreateUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public bool isExist(string username, string email)
        {
            return context.Users.Any(u => u.Username == username || u.Email == email);
        }

        public void UpdateUser(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        public async Task<User?> getUser(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
        }

        private IQueryable<User> getQuery(string? filter)
        {
            IQueryable<User> query = context.Users.Include(u => u.Role).Where(u => u.IsDeleted == false);
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(u => u.Firstname.ToLower().Contains(filter.ToLower().Trim()) || u.Lastname.ToLower().Contains(filter.ToLower().Trim())
                || u.Role.Rolename.Contains(filter.ToLower().Trim()) || u.Email.ToLower().Contains(filter.ToLower().Trim())
                || u.Username.ToLower().Contains(filter.ToLower().Trim()));
            }
            return query;
        }

        public async Task<List<User>> getList(string? filter, int page)
        {
            IQueryable<User> query = getQuery(filter);
            return await query.OrderByDescending(u => u.UpdateAt).Skip(PageSizeConst.MAX_USER_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_USER_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            IQueryable<User> query = getQuery(filter);
            return await query.CountAsync();
        }

        public async Task<User?> getUser(Guid UserID)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.UserId == UserID && u.IsDeleted == false);
        }

        public bool isExist(Guid UserID, string username, string email)
        {
            return context.Users.Any(u => (u.Username == username || u.Email == email) && u.UserId != UserID);
        }
        public void DeleteUser(User user)
        {
            user.IsDeleted = true;
            UpdateUser(user);
        }
        public async Task<List<string>> getEmailAdmins()
        {
            return await context.Users.Where(u => u.RoleId == RoleConst.INT_ROLE_ADMIN).Select(u => u.Email).ToListAsync();
        }

    }
}
