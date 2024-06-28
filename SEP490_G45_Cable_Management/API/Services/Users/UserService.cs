using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.UserDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using DataAccess.Util;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace API.Services.Users
{
    public class UserService : BaseService, IUserService
    {
        private readonly DAOUser daoUser = new DAOUser();

        public UserService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<TokenDTO?>> Login(LoginDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(DTO);
                // if username or password incorrect
                if (user == null)
                {
                    return new ResponseBase<TokenDTO?>(null, "Username or password wrong", (int)HttpStatusCode.Conflict);
                }
                string AccessToken = getAccessToken(user);
                TokenDTO token = new TokenDTO()
                {
                    Access_Token = AccessToken,
                    Role = user.Role.Rolename
                };
                return new ResponseBase<TokenDTO?>(token, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<TokenDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
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
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                //new Claim("username", user.Username),
                new Claim("UserID", user.UserId.ToString()),
/*                new Claim(ClaimTypes.Email, user.Email),
                new Claim("phone",user.Phone == null ? "" : user.Phone),
                new Claim("FirstName",user.Firstname),
                new Claim("LastName",user.Lastname),
                new Claim(ClaimTypes.Role, dic[user.RoleId])*/
            };
            JwtSecurityToken token = new JwtSecurityToken(JWT["Issuer"],
                JWT["Audience"], list, expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // get access token
            return handler.WriteToken(token);
        }
        public async Task<ResponseBase<bool>> Create(UserCreateDTO DTO)
        {
            if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên người dùng không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.Phone.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Số điện thoại không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if user exist
                if (await daoUser.isExist(DTO.UserName, DTO.Email))
                {
                    return new ResponseBase<bool>(false, "Email hoặc username đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
                string newPw = UserUtil.RandomPassword();
                string hashPw = UserUtil.HashPassword(newPw);
                // get body email
                string body = UserUtil.BodyEmailForRegister(newPw);
                // send email
                await UserUtil.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // create
                User user = _mapper.Map<User>(DTO);
                user.UserId = Guid.NewGuid();
                user.Password = hashPw;
                user.RoleId = (int)Common.Enum.Role.Staff;
                user.CreatedAt = DateTime.Now;
                user.CreatedDate = DateTime.Now;
                user.UpdateAt = DateTime.Now;
                user.IsDeleted = false;
                await daoUser.CreateUser(user);
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> ForgotPassword(ForgotPasswordDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(DTO.Email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy email trong hệ thống", (int)HttpStatusCode.NotFound);
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
                return new ResponseBase<bool>(true, "Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> ChangePassword(ChangePasswordDTO DTO, string email)
        {
            try
            {
                User? user = await daoUser.getUser(email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy thông tin của bạn", (int)HttpStatusCode.NotFound);
                }
                // if current password not correct
                if (!user.Password.Equals(UserUtil.HashPassword(DTO.CurrentPassword)))
                {
                    return new ResponseBase<bool>(false, "Mật khẩu hiện tại không chính xác", (int)HttpStatusCode.Conflict);
                }
                // if confirm password not the same as new password
                if (!DTO.ConfirmPassword.Equals(DTO.NewPassword))
                {
                    return new ResponseBase<bool>(false, "Mật khẩu xác nhận không trùng khớp với mật khẩu mới", (int)HttpStatusCode.Conflict);
                }
                user.Password = UserUtil.HashPassword(DTO.NewPassword);
                await daoUser.UpdateUser(user);
                return new ResponseBase<bool>(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<Pagination<UserListDTO>?>> ListPaged(string? filter, int page)
        {
            try
            {
                List<User> list = await daoUser.getList(filter, page);
                List<UserListDTO> DTOs = _mapper.Map<List<UserListDTO>>(list);
                int RowCount = await daoUser.getRowCount(filter);
                Pagination<UserListDTO> result = new Pagination<UserListDTO>(page, RowCount, PageSizeConst.MAX_USER_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<UserListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<UserListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(Guid UserID, UserUpdateDTO DTO)
        {
            try
            {
                User? user = await daoUser.getUser(UserID);
                // if not found
                if (user == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy người dùng", (int)HttpStatusCode.NotFound);
                }
                // if username or email exist
                if (await daoUser.isExist(UserID, DTO.UserName, DTO.Email))
                {
                    return new ResponseBase<bool>(false, "Email hoặc username đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
                if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên người dùng không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.Phone.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Số điện thoại không được để trống", (int)HttpStatusCode.Conflict);
                }
                user.Username = DTO.UserName;
                user.Email = DTO.Email;
                user.Firstname = DTO.FirstName.Trim();
                user.Lastname = DTO.LastName.Trim();
                user.Phone = DTO.Phone;
                user.RoleId = DTO.RoleId;
                user.UpdateAt = DateTime.Now;
                await daoUser.UpdateUser(user);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(Guid UserID, Guid UserLoginID)
        {
            try
            {
                User? user = await daoUser.getUser(UserID);
                // if not found
                if (user == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy user", (int)HttpStatusCode.NotFound);
                }
                // if user login's account
                if (UserID.Equals(UserLoginID))
                {
                    return new ResponseBase<bool>(false, "Bạn không thể xóa tài khoản của mình", (int)HttpStatusCode.NotAcceptable);
                }
                await daoUser.DeleteUser(user);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<UserListDTO>?>> ListWarehouseKeeper()
        {
            try
            {
                List<User> list = await daoUser.getList();
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
                return new ResponseBase<List<UserListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<UserListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
