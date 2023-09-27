using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using DataAccess.Model.Util;
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

        public async Task<ResponseDTO<string>> Login(LoginDTO DTO)
        {
            User? user = await daoUser.getUser(DTO);
            // if username or password incorrect
            if (user == null) {
                return new ResponseDTO<string>("","Username or password wrong", (int) HttpStatusCode.NotAcceptable);
            }
            string AccessToken = getAccessToken(user);
            return new ResponseDTO<string>(AccessToken);
        }

        private string getAccessToken(User user)
        {
            // ---------------- get information of section Jwt ----------------
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection JWT = config.GetSection("Jwt");
            // get credential
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(JWT["key"]);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            // get all role
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
            // get access token
            JwtSecurityToken token = new JwtSecurityToken(JWT["Issuer"],
                JWT["Audience"], list, expires: DateTime.Now.AddDays(14),
                signingCredentials: credentials);
            return handler.WriteToken(token);
        }

        public async Task<ResponseDTO<bool>> Register(RegisterDTO DTO, string password)
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                UserName = DTO.UserName,
                LastName = DTO.LastName.Trim(),
                FirstName = DTO.FirstName.Trim(),
                Email = DTO.Email,
                Password = password,
                Phone = DTO.Phone,
                RoleId = RoleConst.INT_ROLE_STAFF,
                CreatedAt = DateTime.Now,
                UpdateAt = null,
                IsDeleted = false
            };
            // if user exist
            if(await daoUser.isExist(user.UserName, user.Email))
            {
                return new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng", (int) HttpStatusCode.Conflict);
            }
            int number = await daoUser.AddUser(user);
            // if register successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true);
            }
            return new ResponseDTO<bool>(false, "Đăng ký không thành công", (int) HttpStatusCode.Conflict);
        }

    }
}
