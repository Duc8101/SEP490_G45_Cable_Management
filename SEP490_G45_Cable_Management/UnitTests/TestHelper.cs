using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class TestHelper
    {
        public static Guid SimulateUserWithRoleAndId(BaseAPIController controller, string role)
        {
            Guid userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, role),
            new Claim("UserID", userId.ToString()),
            new Claim("FirstName", "First"),
            new Claim("LastName", "Last"),
            new Claim(ClaimTypes.Email, "emailsample@gmail.com")
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            controller.ControllerContext = context;
            return userId;
        }

        public static String SimulateUser(BaseAPIController controller, string role)
        {
            Guid userId = Guid.NewGuid();
            String email = "emailsample@gmail.com";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, role),
            new Claim("UserID", userId.ToString()),
            new Claim("FirstName", "First"),
            new Claim("LastName", "Last"),
            new Claim(ClaimTypes.Email, email)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            controller.ControllerContext = context;
            return email;
        }

        public static void SimulateUserWithRoleWithoutID(BaseAPIController controller, string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Role, role)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            controller.ControllerContext = context;

        }
    }
}
