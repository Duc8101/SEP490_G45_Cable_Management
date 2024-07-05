using Common.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UnitTests.Helper
{
    public class TestHelper
    {

        public static Dictionary<string, object> SimulateUserWithRoleAndId(string role)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Guid userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim("id", userId.ToString()),
                new Claim("FirstName", "First"),
                new Claim("LastName", "Last"),
                new Claim(ClaimTypes.Email, "emailsample@gmail.com")
            }));
            dic["userId"] = userId;
            dic["user"] = user;
            return dic;
        }

        public static string SimulateToken(int roleId)
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                RoleId = roleId,
                Firstname = "First",
                Lastname = "Last",
                Email = "emailsample@gmail.com"
            };
            // get credential
            byte[] key = Encoding.UTF8.GetBytes("Yh2k7QSu4l8CZg5p6X3Pna9L0Miy4D3Bvt0JVr87UcOj69Kqw5R2Nmf4FWs03Hdx");
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role, user.Role.Rolename),
                new Claim("FirstName", user.Firstname),
                new Claim("LastName", user.Lastname),
            };
            JwtSecurityToken token = new JwtSecurityToken("JWTAuthenticationServer",
                "JWTServicePostmanClient", list, expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // get access token
            return handler.WriteToken(token);
        }
    }
}
