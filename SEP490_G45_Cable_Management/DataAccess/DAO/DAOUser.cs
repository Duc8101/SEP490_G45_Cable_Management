using Common.DTO.UserDTO;
using Common.Entity;
using Common.Enum;
using DataAccess.DBContext;
using DataAccess.Helper;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOUser : BaseDAO
    {
        public DAOUser(CableManagementContext context) : base(context)
        {
        }

        public User? getUser(LoginDTO DTO)
        {
            User? user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == DTO.Username && u.IsDeleted == false);
            // if username or password invalid
            if (user == null || string.Compare(user.Password, UserHelper.HashPassword(DTO.Password), false) != 0)
            {
                return null;
            }
            return user;
        }

        public bool isExist(string username, string email)
        {
            return _context.Users.Any(u => u.Username == username || u.Email == email);
        }

        public void CreateUser(User user)
        {
            _context.Users.AddAsync(user);
            Save();
        }

        public User? getUser(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.IsDeleted == false);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            Save();
        }

        private IQueryable<User> getQuery(string? filter)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role).Where(u => u.IsDeleted == false);
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(u => u.Firstname.ToLower().Contains(filter.ToLower().Trim()) || u.Lastname.ToLower().Contains(filter.ToLower().Trim())
                || u.Role.Rolename.Contains(filter.ToLower().Trim()) || u.Email.ToLower().Contains(filter.ToLower().Trim())
                || u.Username.ToLower().Contains(filter.ToLower().Trim()));
            }
            return query;
        }
        // get all user
        public List<User> getListUser(string? filter, int page)
        {
            IQueryable<User> query = getQuery(filter);
            return query.OrderByDescending(u => u.UpdateAt).Skip((int)PageSize.Size * (page - 1))
                .Take((int)PageSize.Size).ToList();
        }

        public int getRowCount(string? filter)
        {
            IQueryable<User> query = getQuery(filter);
            return query.Count();
        }

        public User? getUser(Guid userId)
        {
            return _context.Users.SingleOrDefault(u => u.UserId == userId && u.IsDeleted == false);
        }

        public bool isExist(Guid userId, string username, string email)
        {
            return _context.Users.Any(u => (u.Username == username || u.Email == email) && u.UserId != userId);
        }
        public void DeleteUser(User user)
        {
            user.IsDeleted = true;
            UpdateUser(user);
        }

        public List<User> getListWarehouseKeeper()
        {
            IQueryable<User> query = getQuery(null);
            return query.OrderByDescending(u => u.UpdateAt).Where(u => u.RoleId == (int)Common.Enum.Role.Warehouse_Keeper)
                .ToList();
        }

        public List<string> getEmailAdmins()
        {
            return _context.Users.Where(u => u.RoleId == (int)Common.Enum.Role.Admin && u.IsDeleted == false)
                .Select(u => u.Email).ToList();
        }

    }
}
