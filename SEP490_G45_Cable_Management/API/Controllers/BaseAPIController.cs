using Common.Enums;
using DataAccess.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class BaseAPIController : ControllerBase
    {

        private Claim? getClaim(string type)
        {
            return User.Claims.FirstOrDefault(c => c.Type == type);
        }

        private protected bool isAdmin()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == Roles.Admin.getDescription();
        }

        private protected bool isLeader()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == Roles.Leader.getDescription();
        }

        private protected bool isWarehouseKeeper()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == Roles.Warehouse_Keeper.getDescription();
        }

        private protected bool isStaff()
        {
            Claim? claim = getClaim(ClaimTypes.Role);
            return claim != null && claim.Value == Roles.Staff.getDescription();
        }

        private protected Guid? getUserId()
        {
            Claim? claim = getClaim("id");
            return claim == null ? null : Guid.Parse(claim.Value);
        }

        private protected string? getEmail()
        {
            Claim? claim = getClaim(ClaimTypes.Email);
            return claim?.Value;
        }

        private protected string? getFirstName()
        {
            Claim? claim = getClaim("FirstName");
            return claim?.Value;
        }

        private protected string? getLastName()
        {
            Claim? claim = getClaim("LastName");
            return claim?.Value;
        }
    }
}
