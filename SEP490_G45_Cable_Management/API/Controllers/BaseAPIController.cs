using DataAccess.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BaseAPIController : ControllerBase
    {
        private User? getUser()
        {
            return (User?)HttpContext.Items["user"];
        }

        internal bool isAdmin()
        {
            User? user = getUser();
            return user != null && user.RoleId == (int)DataAccess.Enum.Role.Admin;
        }

        internal bool isLeader()
        {
            User? user = getUser();
            return user != null && user.RoleId == (int)DataAccess.Enum.Role.Leader;
        }

        internal bool isWarehouseKeeper()
        {
            User? user = getUser();
            return user != null && user.RoleId == (int)DataAccess.Enum.Role.Warehouse_Keeper;
        }

        internal Guid? getUserID()
        {
            User? user = getUser();
            return user?.UserId;
        }

        internal string? getEmail()
        {
            User? user = getUser();
            return user?.Email;
        }

        internal string? getFirstName()
        {
            User? user = getUser();
            return user?.Firstname;
        }
        internal string? getLastName()
        {
            User? user = getUser();
            return user?.Lastname;
        }
    }
}
