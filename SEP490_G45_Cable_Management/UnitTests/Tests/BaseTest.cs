using API;
using AutoMapper;
using Common.Entity;
using Common.Enums;
using DataAccess.Configuration;
using DataAccess.DBContext;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq.Protected;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace UnitTests.Tests
{
    public class BaseTest
    {
        private protected Mock<CableManagementContext> mockDbContext;
        private protected IMapper mapper;

        [SetUp]
        public virtual void SetUp()
        {
            mockDbContext = new Mock<CableManagementContext>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();
        }

        private protected Mock<HttpMessageHandler> getHttpMessageHandler(string content)
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
                  // Content = new StringContent(content)
               });
            return handler;
        }

        private protected ClaimsPrincipal SimulateUser(Roles role)
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                RoleId = (int)role,
                Firstname = "First",
                Lastname = "Last",
                Email = "emailsample@gmail.com"
            };
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role, role.getDescription()),
                new Claim("FirstName", user.Firstname),
                new Claim("LastName", user.Lastname),
            };
            return new ClaimsPrincipal(new ClaimsIdentity(list));
        }

        private protected string SimulateToken(Roles role)
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
            byte[] key = Encoding.UTF8.GetBytes(ConfigData.JwtKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role, role.getDescription()),
                new Claim("FirstName", user.Firstname),
                new Claim("LastName", user.Lastname),
            };
            JwtSecurityToken token = new JwtSecurityToken(ConfigData.JwtIssuer,
                ConfigData.JwtAudience, list, expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // get access token
            return handler.WriteToken(token);
        }

        private protected async Task<HttpResponseMessage> Get(HttpClient client, string url, params KeyValuePair<string, object>[] parameters)
        {
            StringBuilder param = new StringBuilder();
            if (parameters.Length > 0)
            {
                param.Append("?");
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == 0)
                    {
                        param.Append(parameters[i].Key + "=" + parameters[i].Value);
                    }
                    else
                    {
                        param.Append("&" + parameters[i].Key + "=" + parameters[i].Value);
                    }
                }
            }
            url = url + param;
            return await client.GetAsync(url);
        }

        private protected async Task<HttpResponseMessage> Post<T>(HttpClient client, string url, T obj)
        {
            string body = JsonSerializer.Serialize(obj);
            StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
            return await client.PostAsync(url, content);
        }


    }
}
