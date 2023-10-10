using API.Services;
using DataAccess.DTO.UserDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Entity;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : BaseAPIController
    {
        private readonly UserService service = new UserService();

        [HttpPost]
        public async Task<ResponseDTO<string?>> Login(LoginDTO DTO)
        {
            return await service.Login(DTO);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<UserListDTO>?>> List(string? filter, int page)
        {
            // if admin
            if (isAdmin())
            {
                return await service.List(filter, page);
            }
            return new ResponseDTO<PagedResultDTO<UserListDTO>?>(null, "Bạn không có quyền truy cập" , (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(UserCreateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{UserID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid UserID, UserUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(UserID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{UserID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid UserID)
        {
            // if admin
            if (isAdmin())
            {
                string? UserLoginID = getUserID();
                // if not found
                if(UserLoginID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID", (int)HttpStatusCode.NotFound);
                }
                return await service.Delete(UserID, Guid.Parse(UserLoginID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public async Task<ResponseDTO<bool>> ForgotPassword(ForgotPasswordDTO DTO)
        {
            return await service.ForgotPassword(DTO);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> ChangePassword(ChangePasswordDTO DTO)
        {
            // if not login 
            if (isGuest())
            {
                return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
            }
            string? email = getEmail();
            // if not found email
            if(email == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy email. Cần xác minh lại thông tin đăng nhập", (int) HttpStatusCode.NotFound);
            }
            return await service.ChangePassword(DTO, email);
        }
    }
}
