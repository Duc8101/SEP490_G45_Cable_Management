using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.UserDTO;
using Common.Entity;
using Common.Enums;
using Common.Paginations;
using DataAccess.DAO;
using DataAccess.Helper;
using System.Net;

namespace API.Services.Users
{
    public class UserService : BaseService, IUserService
    {
        private readonly DAOUser _daoUser;
        public UserService(IMapper mapper, DAOUser daoUser) : base(mapper)
        {
            _daoUser = daoUser;
        }

        public ResponseBase Login(LoginDTO DTO)
        {
            try
            {
                User? user = _daoUser.getUser(DTO);
                // if username or password incorrect
                if (user == null)
                {
                    return new ResponseBase("Username or password wrong", (int)HttpStatusCode.Conflict);
                }
                string AccessToken = UserHelper.GenerateToken(user);
                TokenDTO token = new TokenDTO()
                {
                    Access_Token = AccessToken,
                    Role = user.Role.Rolename
                };
                return new ResponseBase(token);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase> Create(UserCreateDTO DTO)
        {
            if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên người dùng không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.Phone.Trim().Length == 0)
            {
                return new ResponseBase(false, "Số điện thoại không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if user exist
                if (_daoUser.isExist(DTO.UserName, DTO.Email))
                {
                    return new ResponseBase(false, "Email hoặc username đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
                string newPw = UserHelper.RandomPassword();
                string hashPw = UserHelper.HashPassword(newPw);
                // get body email
                string body = UserHelper.BodyEmailForRegister(newPw);
                // send email
                await UserHelper.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // create
                User user = _mapper.Map<User>(DTO);
                user.UserId = Guid.NewGuid();
                user.Password = hashPw;
                user.RoleId = (int)Roles.Staff;
                user.CreatedAt = DateTime.Now;
                user.CreatedDate = DateTime.Now;
                user.UpdateAt = DateTime.Now;
                user.IsDeleted = false;
                _daoUser.CreateUser(user);
                return new ResponseBase(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase> ForgotPassword(ForgotPasswordDTO DTO)
        {
            try
            {
                User? user = _daoUser.getUser(DTO.Email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseBase(false, "Không tìm thấy email trong hệ thống", (int)HttpStatusCode.NotFound);
                }
                // get new password
                string newPw = UserHelper.RandomPassword();
                string hashPw = UserHelper.HashPassword(newPw);
                // get body email
                string body = UserHelper.BodyEmailForForgetPassword(newPw);
                // send email
                await UserHelper.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // update user
                user.Password = hashPw;
                _daoUser.UpdateUser(user);
                return new ResponseBase(true, "Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ChangePassword(ChangePasswordDTO DTO, string email)
        {
            try
            {
                User? user = _daoUser.getUser(email);
                // if email not exist
                if (user == null)
                {
                    return new ResponseBase(false, "Không tìm thấy thông tin của bạn", (int)HttpStatusCode.NotFound);
                }
                // if current password not correct
                if (!user.Password.Equals(UserHelper.HashPassword(DTO.CurrentPassword)))
                {
                    return new ResponseBase(false, "Mật khẩu hiện tại không chính xác", (int)HttpStatusCode.Conflict);
                }
                // if confirm password not the same as new password
                if (!DTO.ConfirmPassword.Equals(DTO.NewPassword))
                {
                    return new ResponseBase(false, "Mật khẩu xác nhận không trùng khớp với mật khẩu mới", (int)HttpStatusCode.Conflict);
                }
                user.Password = UserHelper.HashPassword(DTO.NewPassword);
                _daoUser.UpdateUser(user);
                return new ResponseBase(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListUserPaged(string? filter, int page)
        {
            try
            {
                List<User> list = _daoUser.getListUser(filter, page);
                List<UserListDTO> DTOs = _mapper.Map<List<UserListDTO>>(list);
                int rowCount = _daoUser.getRowCount(filter);
                Pagination<UserListDTO> result = new Pagination<UserListDTO>()
                {
                    CurrentPage = page,
                    List = DTOs,
                    RowCount = rowCount
                };
                return new ResponseBase(result);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(Guid userId, UserUpdateDTO DTO)
        {
            try
            {
                User? user = _daoUser.getUser(userId);
                // if not found
                if (user == null)
                {
                    return new ResponseBase(false, "Không tìm thấy người dùng", (int)HttpStatusCode.NotFound);
                }
                // if username or email exist
                if (_daoUser.isExist(userId, DTO.UserName, DTO.Email))
                {
                    return new ResponseBase(false, "Email hoặc username đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
                if (DTO.FirstName.Trim().Length == 0 || DTO.LastName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên người dùng không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.Phone.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Số điện thoại không được để trống", (int)HttpStatusCode.Conflict);
                }
                user.Username = DTO.UserName;
                user.Email = DTO.Email;
                user.Firstname = DTO.FirstName.Trim();
                user.Lastname = DTO.LastName.Trim();
                user.Phone = DTO.Phone;
                user.RoleId = DTO.RoleId;
                user.UpdateAt = DateTime.Now;
                _daoUser.UpdateUser(user);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(Guid userId, Guid userLoginId)
        {
            try
            {
                User? user = _daoUser.getUser(userId);
                // if not found
                if (user == null)
                {
                    return new ResponseBase(false, "Không tìm thấy user", (int)HttpStatusCode.NotFound);
                }
                // if user login's account
                if (userId.Equals(userLoginId))
                {
                    return new ResponseBase(false, "Bạn không thể xóa tài khoản của mình", (int)HttpStatusCode.NotAcceptable);
                }
                _daoUser.DeleteUser(user);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListWarehouseKeeper()
        {
            try
            {
                List<User> list = _daoUser.getListWarehouseKeeper();
                List<UserListDTO> result = _mapper.Map<List<UserListDTO>>(list);
                return new ResponseBase(result);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
