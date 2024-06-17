using API.Provider;
using Common.Base;
using Common.Entity;
using DataAccess.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace API.Attributes
{
    public class RoleAttribute : Attribute, IActionFilter
    {
        private Common.Enum.Role[] Roles { get; set; }

        public RoleAttribute(params Common.Enum.Role[] roles)
        {
            Roles = roles;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var accessor = StaticServiceProvider.Provider.GetService<IHttpContextAccessor>();
            var dbContext = accessor?.HttpContext?.RequestServices.GetService<CableManagementContext>();
            if (dbContext == null)
            {
                ResponseBase<object?> result = new ResponseBase<object?>(null, "Có lỗi xảy ra khi check role", (int)HttpStatusCode.InternalServerError);
                context.Result = new JsonResult(result)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }
            else
            {
                // ------------------ get token ----------------------------------------
                string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                // ------------------ get information user from token ------------------
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(token);
                string UserId = tokenS.Claims.First(t => t.Type == "UserID").Value;
                Guid userId = Guid.Parse(UserId);
                User? user = dbContext.Users.SingleOrDefault(u => u.UserId == userId && u.IsDeleted == false);
                if (user == null)
                {
                    ResponseBase<object?> result = new ResponseBase<object?>(null, "Not found user", (int)HttpStatusCode.NotFound);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }
                else if (!Roles.Contains((Common.Enum.Role)user.RoleId))
                {
                    ResponseBase<object?> result = new ResponseBase<object?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                    };
                }
            }
        }
    }
}
