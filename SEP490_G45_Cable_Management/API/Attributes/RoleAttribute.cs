using Common.Base;
using Common.Entity;
using Common.Enums;
using DataAccess.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

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
            DAOUser daoUser = context.HttpContext.RequestServices.GetRequiredService<DAOUser>();
            // ------------------ get token ----------------------------------------
            string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // ------------------ get information user from token ------------------
            var handler = new JwtSecurityTokenHandler();
            var security = handler.ReadJwtToken(token);
            Claim? claim = security.Claims.FirstOrDefault(t => t.Type == "id");
            if (claim == null)
            {
                ResponseBase result = new ResponseBase("Not found user id based on token. Please check information!!", (int)HttpStatusCode.NotFound);
                context.Result = new JsonResult(result)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
            else
            {
                Guid userId = Guid.Parse(claim.Value);
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
