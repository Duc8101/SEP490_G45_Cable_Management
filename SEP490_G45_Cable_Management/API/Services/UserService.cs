using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using API.Model.DAO;
using API.Model.Util;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
            return new ResponseDTO<string>(AccessToken,"");
        }

        private string getAccessToken(User user)
        {
            // ---------------- get information of section Jwt ----------------
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection JWT = config.GetSection("Jwt");
            // get credential
            byte[] key = Encoding.UTF8.GetBytes(JWT["key"]);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            // get all role
            Dictionary<int, string> dic = RoleConst.getList();
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("username", user.Username),
                new Claim("UserID", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("phone",user.Phone == null ? "" : user.Phone),
                new Claim("FirstName",user.Firstname),
                new Claim("LastName",user.Lastname),
                new Claim(ClaimTypes.Role, dic[user.RoleId])
            };
            JwtSecurityToken token = new JwtSecurityToken(JWT["Issuer"],
                JWT["Audience"], list, expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // get access token
            return handler.WriteToken(token);
        }

        public async Task<ResponseDTO<bool>> Create(UserCreateDTO DTO)
        {
            try
            {
                string newPw = UserUtil.RandomPassword();
                string hashPw = UserUtil.HashPassword(newPw);
                // get body email
                string body = UserUtil.BodyEmailForRegister(newPw);
                // send email
                await UserUtil.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // create
                User user = new User()
                {
                    UserId = Guid.NewGuid(),
                    Username = DTO.UserName,
                    Lastname = DTO.LastName.Trim(),
                    Firstname = DTO.FirstName.Trim(),
                    Email = DTO.Email,
                    Password = hashPw,
                    Phone = DTO.Phone,
                    RoleId = RoleConst.INT_ROLE_STAFF,
                    CreatedAt = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                // if user exist
                if (await daoUser.isExist(user.Username, user.Email))
                {
                    return new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng", (int) HttpStatusCode.NotAcceptable);
                }
                int number = await daoUser.CreateUser(user);
                // if create successful
                if (number > 0)
                {
                    return new ResponseDTO<bool>(true, "Tạo thành công");
                }
                return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                // if send email failed, throw message
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> ForgotPassword(ForgotPasswordDTO DTO)
        {
            User? user = await daoUser.getUser(DTO.Email);
            // if email not exist
            if (user == null)
            {
                return new ResponseDTO<bool>(false, "Email này chưa được đăng ký", (int) HttpStatusCode.NotFound);
            }

            try
            {
                // get new password
                string newPw = UserUtil.RandomPassword();
                string hashPw = UserUtil.HashPassword(newPw);
                // get body email
                string body = UserUtil.BodyEmailForForgetPassword(newPw);
                // send email
                await UserUtil.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // update user
                user.Password = hashPw;
                int number = await daoUser.UpdateUser(user);
                // if update successful
                if (number > 0)
                {
                    return new ResponseDTO<bool>(true, "Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn");
                }
                return new ResponseDTO<bool>(false, "Không đổi được mật khẩu", (int) HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                // if send email failed, throw message
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> ChangePassword(ChangePasswordDTO DTO, string email)
        {
            User? user = await daoUser.getUser(email);
            // if email not exist
            if (user == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy thông tin", (int) HttpStatusCode.NotFound);
            }
            // if current password not correct
            if (!user.Password.Equals(UserUtil.HashPassword(DTO.CurrentPassword)))
            {
                return new ResponseDTO<bool>(false, "Mật khẩu hiện tại không chính xác", (int) HttpStatusCode.NotAcceptable);
            }
            // if confirm password not the same as new password
            if (!DTO.ConfirmPassword.Equals(DTO.NewPassword))
            {
                return new ResponseDTO<bool>(false, "Mật khẩu xác nhận không trùng khớp với mật khẩu mới", (int) HttpStatusCode.NotAcceptable);
            }
            user.Password = UserUtil.HashPassword(DTO.NewPassword);
            int number = await daoUser.UpdateUser(user); 
            // if change successful
            if (number > 0) { 
                return new ResponseDTO<bool>(true, "Đổi mật khẩu thành công"); 
            }
            return new ResponseDTO<bool>(false, "Đổi mật khẩu thất bại", (int) HttpStatusCode.Conflict);
        }

        private async Task<List<UserListDTO>> getList(string? filter, int page)
        {
            List<User> list = await daoUser.getList(filter, page);
            List<UserListDTO> result = new List<UserListDTO>();
            foreach (User user in list)
            {
                UserListDTO DTO = new UserListDTO()
                {
                    UserId = user.UserId,
                    UserName = user.Username,
                    FirstName = user.Firstname,
                    LastName = user.Lastname,
                    Email = user.Email,
                    Phone = user.Phone,
                    RoleName = user.Role.Rolename
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<PagedResultDTO<UserListDTO>> List(string? filter, int page)
        {
            List<UserListDTO> list = await getList(filter, page);
            int RowCount = await daoUser.getRowCount(filter);
            return new PagedResultDTO<UserListDTO>(page, RowCount, PageSizeConst.MAX_USER_LIST_IN_PAGE, list);
        }

        public async Task<ResponseDTO<bool>> Update(Guid UserID, UserUpdateDTO DTO)
        {
            User? user = await daoUser.getUser(UserID);
            // if not found
            if(user == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy người dùng" , (int) HttpStatusCode.NotFound);
            }
            // if username or email exist
            if(await daoUser.isExist(UserID, DTO.UserName, DTO.Email))
            {
                return new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng", (int) HttpStatusCode.NotAcceptable);
            }
            user.Username = DTO.UserName;
            user.Email = DTO.Email;
            user.Firstname = DTO.FirstName.Trim();
            user.Lastname = DTO.LastName.Trim();
            user.Phone = DTO.Phone;
            user.RoleId = DTO.RoleId;
            user.UpdateAt = DateTime.Now;
            int number = await daoUser.UpdateUser(user);
            // if update successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Không chỉnh sửa được", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(Guid UserID, Guid UserLoginID)
        {
            User? user = await daoUser.getUser(UserID);
            // if not found
            if(user == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy user", (int) HttpStatusCode.NotFound);
            }
            // if user login's account
            if(UserID.Equals(UserLoginID))
            {
                return new ResponseDTO<bool>(false, "Bạn không thể xóa tài khoản của mình", (int) HttpStatusCode.NotFound);
            }
            int number = await daoUser.DeleteUser(user);
            // if delete suceesful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }

    }
}
