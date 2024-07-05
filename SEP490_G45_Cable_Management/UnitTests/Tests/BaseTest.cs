using Common.Entity;
using Microsoft.IdentityModel.Tokens;
using Moq.Protected;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UnitTests.Tests
{
    public class BaseTest
    {
        internal Mock<HttpMessageHandler> getHttpMessageHandler(HttpStatusCode code, string content)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
               .Protected() // Use Protected() to access protected members of HttpMessageHandler
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = code,
                   Content = new StringContent(content)
               });
            return handler;
        }

        internal string SimulateToken(Common.Enum.Role role)
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                RoleId = (int)role,
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
                new Claim(ClaimTypes.Role, role.ToString()),
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

        internal async Task<HttpResponseMessage> Get(HttpClient client, string url, params KeyValuePair<string, object>[] parameters)
        {
            string param = "";
            if (parameters.Length > 0)
            {
                param = "?";
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == 0)
                    {
                        param = param + parameters[i].Key + "=" + parameters[i].Value;
                    }
                    else
                    {
                        param = param + "&" + parameters[i].Key + "=" + parameters[i].Value;
                    }
                }
            }
            url = url + param;
            return await client.GetAsync(url);
        }

    }
}
