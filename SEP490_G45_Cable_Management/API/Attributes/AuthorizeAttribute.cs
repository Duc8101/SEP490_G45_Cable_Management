using DataAccess.DTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace API.Attributes
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // get user
            User? user = (User?)context.HttpContext.Items["user"];
            if (user == null)
            {
                ResponseDTO<object?> result = new ResponseDTO<object?>(null, "Unauthorized", (int)HttpStatusCode.Unauthorized);
                context.Result = new JsonResult(result)
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                };
            }
        }
    }
}
