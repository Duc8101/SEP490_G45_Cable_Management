using API.Provider;
using Common.Base;
using Common.Entity;
using Common.Enums;
using DataAccess.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace API.Attributes
{
    public class RoleAttribute : Attribute, IActionFilter
    {
        public Roles[] Roles { get; }

        public RoleAttribute(params Roles[] roles)
        {
            Roles = roles;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var accessor = StaticServiceProvider.Provider.GetService<IHttpContextAccessor>();
            var daoUser = accessor?.HttpContext?.RequestServices.GetService<DAOUser>();
            if (daoUser == null)
            {
                ResponseBase result = new ResponseBase("Không lấy dc giá trị DAOUser", (int)HttpStatusCode.InternalServerError);
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
                string UserId = tokenS.Claims.First(t => t.Type == "id").Value;
                Guid userId = Guid.Parse(UserId);
                User? user = daoUser.getUser(userId);
                if (user == null)
                {
                    ResponseBase result = new ResponseBase("Not found user", (int)HttpStatusCode.NotFound);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }
                else if (!Roles.Contains((Roles)user.RoleId))
                {
                    ResponseBase result = new ResponseBase("Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
                    context.Result = new JsonResult(result)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                    };
                }
            }
        }
    }
}
