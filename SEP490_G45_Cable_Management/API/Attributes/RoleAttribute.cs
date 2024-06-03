using API.Model;
using API.Provider;
using DataAccess.DTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace API.Attributes
{
    public class RoleAttribute : Attribute, IActionFilter
    {
        private DataAccess.Enum.Role[] Roles { get; set; }

        public RoleAttribute(params DataAccess.Enum.Role[] roles)
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
                ResponseDTO<object?> result = new ResponseDTO<object?>(null, "Có lỗi xảy ra khi check role", (int)HttpStatusCode.InternalServerError);
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
                User? user = dbContext.Users.Find(userId);
                if (user == null)
                {
                    ResponseDTO<object?> result = new ResponseDTO<object?>(null, "Not found user", (int)HttpStatusCode.NotFound);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }
                else if (!Roles.Contains((DataAccess.Enum.Role)user.RoleId))
                {
                    ResponseDTO<object?> result = new ResponseDTO<object?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                    };
                }
            }
        }
    }
}
