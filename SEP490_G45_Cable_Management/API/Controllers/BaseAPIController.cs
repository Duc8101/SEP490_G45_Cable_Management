using DataAccess.Const;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class BaseAPIController : ControllerBase
    {
        private string? getRole()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim == null ? null : claim.Value;
        }

        protected bool isAdmin()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_ADMIN);
        }
        protected bool isStaff()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_STAFF);
        }

        protected bool isLeader()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_LEADER);
        }

        protected bool isWarehouseKeeper()
        {
            string? role = getRole();
            return role != null && role.Equals(RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);
        }

        private Claim? getClaim(string type)
        {
            return User.Claims.Where(c => c.Type == type).FirstOrDefault();
        }

        /*protected List<Claim> getListClaim()
        {
            return User.Claims.ToList();
        }*/

        protected string? getUserID()
        {
            Claim? claim = getClaim("UserID");
            return claim == null ? null : claim.Value;
        }

        protected bool isGuest()
        {
            string? role = getRole();
            return role == null;
        }

        protected string? getEmail()
        {
            Claim? claim = getClaim(ClaimTypes.Email);
            return claim == null ? null : claim.Value;
        }
    }
}
