using DataAccess.Const;
using DataAccess.DTO.CommonDTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class UserService
    {
        private readonly DAOUser daoUser = new DAOUser();

        public async Task<BaseResponseDTO<TokenResponse>> Login(LoginDTO DTO)
        {
            User? user = await daoUser.getUser(DTO);
            // if username or password incorrect
            if (user == null) {
                return new BaseResponseDTO<TokenResponse>("Username or password wrong", StatusCodes.Status401Unauthorized);
            }
            string AccessToken = getAccessToken(user);
            return new BaseResponseDTO<TokenResponse>(new TokenResponse { Access_Token = AccessToken }, string.Empty, (int) HttpStatusCode.OK);
        }

        private string getAccessToken(User user)
        {
            // ---------------- get information of section Jwt ----------------
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection JWT = config.GetSection("Jwt");
            // ---------------- generate token ----------------
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.UTF8.GetBytes(JWT["key"]);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(tokenKey);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            Dictionary<int, string> dic = RoleConst.getList();
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("username", user.UserName),
                new Claim("UserID", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("phone",user.Phone),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
                new Claim(ClaimTypes.Role, dic[user.RoleId])
            };
            JwtSecurityToken token = new JwtSecurityToken(JWT["Issuer"],
                JWT["Audience"], list, expires: DateTime.Now.AddDays(14),
                signingCredentials: credentials);
            return handler.WriteToken(token);
        }
    }
}
