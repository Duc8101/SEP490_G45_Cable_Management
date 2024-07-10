using Common.Const;
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

        internal bool isAdmin()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == RoleConst.Admin.ToString();
        }

        internal bool isLeader()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == RoleConst.Leader.ToString();
        }

        internal bool isWarehouseKeeper()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == RoleConst.Warehouse_Keeper.ToString();
        }

        internal bool isStaff()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == RoleConst.Staff.ToString();
        }

        internal Guid? getUserId()
        {
            Claim? claim = getClaim("id");
            return claim == null ? null : Guid.Parse(claim.Value);
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
