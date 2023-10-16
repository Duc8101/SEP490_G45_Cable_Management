using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using API.Model;
using API.Model.DAO;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly DAOUser daoUser = new DAOUser();
        public async Task<ResponseDTO<string?>> Login(LoginDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(DTO);
                // if username or password incorrect
                if (user == null)
                {
                    return new ResponseDTO<string?>(null, "Username or password wrong", (int) HttpStatusCode.NotAcceptable);
                }
                string AccessToken = getAccessToken(user);
                return new ResponseDTO<string?>(AccessToken, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<string?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
           
        }

        private string getAccessToken(User user)
        {
            // ---------------- get information of section Jwt ----------------
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection JWT = config.GetSection("Jwt");
            // get credential
            byte[] key = Encoding.UTF8.GetBytes(JWT["Key"]);
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
            if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên người dùng không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.Phone.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Số điện thoại không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {     
                // if user exist
                if (await daoUser.isExist(DTO.UserName, DTO.Email))
                {
                    return new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
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
                await daoUser.CreateUser(user);
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> ForgotPassword(ForgotPasswordDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(DTO.Email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy email trong hệ thống", (int)HttpStatusCode.NotFound);
                }
                // get new password
                string newPw = UserUtil.RandomPassword();
                string hashPw = UserUtil.HashPassword(newPw);
                // get body email
                string body = UserUtil.BodyEmailForForgetPassword(newPw);
                // send email
                await UserUtil.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // update user
                user.Password = hashPw;
                await daoUser.UpdateUser(user);
                return new ResponseDTO<bool>(true, "Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> ChangePassword(ChangePasswordDTO DTO, string email)
        {
            try
            {
                User? user = await daoUser.getUser(email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy thông tin của bạn", (int) HttpStatusCode.NotFound);
                }
                // if current password not correct
                if (!user.Password.Equals(UserUtil.HashPassword(DTO.CurrentPassword)))
                {
                    return new ResponseDTO<bool>(false, "Mật khẩu hiện tại không chính xác", (int) HttpStatusCode.Conflict);
                }
                // if confirm password not the same as new password
                if (!DTO.ConfirmPassword.Equals(DTO.NewPassword))
                {
                    return new ResponseDTO<bool>(false, "Mật khẩu xác nhận không trùng khớp với mật khẩu mới", (int) HttpStatusCode.Conflict);
                }
                user.Password = UserUtil.HashPassword(DTO.NewPassword);
                await daoUser.UpdateUser(user);
                return new ResponseDTO<bool>(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
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

        public async Task<ResponseDTO<PagedResultDTO<UserListDTO>?>> List(string? filter, int page)
        {
            try
            {
                List<UserListDTO> list = await getList(filter, page);
                int RowCount = await daoUser.getRowCount(filter);
                PagedResultDTO<UserListDTO> result = new PagedResultDTO<UserListDTO>(page, RowCount, PageSizeConst.MAX_USER_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<UserListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<UserListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Update(Guid UserID, UserUpdateDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(UserID);
                // if not found
                if (user == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy người dùng", (int)HttpStatusCode.NotFound);
                }
                // if username or email exist
                if (await daoUser.isExist(UserID, DTO.UserName, DTO.Email))
                {
                    return new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng", (int) HttpStatusCode.Conflict);
                }
                if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên người dùng không được để trống", (int) HttpStatusCode.Conflict);
                }
                if (DTO.Phone.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Số điện thoại không được để trống", (int) HttpStatusCode.Conflict);
                }
                user.Username = DTO.UserName;
                user.Email = DTO.Email;
                user.Firstname = DTO.FirstName.Trim();
                user.Lastname = DTO.LastName.Trim();
                user.Phone = DTO.Phone;
                user.RoleId = DTO.RoleId;
                user.UpdateAt = DateTime.Now;
                await daoUser.UpdateUser(user);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Delete(Guid UserID, Guid UserLoginID)
        {
            try
            {
                User? user = await daoUser.getUser(UserID);
                // if not found
                if (user == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy user", (int) HttpStatusCode.NotFound);
                }
                // if user login's account
                if (UserID.Equals(UserLoginID))
                {
                    return new ResponseDTO<bool>(false, "Bạn không thể xóa tài khoản của mình", (int) HttpStatusCode.NotAcceptable);
                }
                await daoUser.DeleteUser(user);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }    
        }

    }
}
