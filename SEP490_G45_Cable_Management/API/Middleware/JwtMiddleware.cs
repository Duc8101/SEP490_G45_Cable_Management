using DataAccess.DBContext;
using System.IdentityModel.Tokens.Jwt;

namespace API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, CableManagementContext dbContext)
        {
            // get token login
            string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // if get token successful
            if(token != null)
            {
                try
                {
                    // ------------------ get information user from token ------------------
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadJwtToken(token);
                    string UserId = tokenS.Claims.First(t => t.Type == "UserID").Value;
                    Guid userId = Guid.Parse(UserId);
                    context.Items["user"] = dbContext.Users.SingleOrDefault(u => u.UserId == userId && u.IsDeleted == false);
                }
                catch
                {
                    // do nothing
                }              
            }
            await _next(context);
        }
    }
}
