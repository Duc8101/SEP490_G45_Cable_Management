using DataAccess.Const;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class BaseAPIController : ControllerBase
    {
        private Claim? getClaim(string type)
        {
            return User.Claims.Where(c => c.Type == type).FirstOrDefault();
        }
        private string? getRole()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim == null ? null : claim.Value;
        }

        internal bool isAdmin()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_ADMIN);
        }
        internal bool isStaff()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_STAFF);
        }

        internal bool isLeader()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_LEADER);
        }

        internal bool isWarehouseKeeper()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);
        }

        internal string? getUserID()
        {
            Claim? claim = getClaim("UserID");
            return claim == null ? null : claim.Value;
        }

        internal string? getEmail()
        {
            Claim? claim = getClaim(ClaimTypes.Email);
            return claim == null ? null : claim.Value;
        }

        internal string? getFirstName()
        {
            Claim? claim = getClaim("FirstName");
            return claim == null ? null : claim.Value;
        }
        internal string? getLastName()
        {
            Claim? claim = getClaim("LastName");
            return claim == null ? null : claim.Value;
        }
    }
}
