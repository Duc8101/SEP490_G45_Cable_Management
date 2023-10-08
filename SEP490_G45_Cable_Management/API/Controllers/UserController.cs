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

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : BaseAPIController
    {
        private readonly UserService service = new UserService();

        [HttpPost]
        public async Task<ResponseDTO<string>> Login(LoginDTO DTO)
        {
            return await service.Login(DTO);
        }

        [HttpGet]
        //[Authorize]
        public async Task<PagedResultDTO<UserListDTO>> List(string? filter, int page)
        {
            // if admin
            //if (isAdmin())
            //{
                return await service.List(filter, page);
            //}
            //throw new UnauthorizedAccessException();
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
            throw new UnauthorizedAccessException();
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
            throw new UnauthorizedAccessException();
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
                    throw new ApplicationException();
                }
                return await service.Delete(UserID, Guid.Parse(UserLoginID));
            }
            throw new UnauthorizedAccessException();
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
                throw new UnauthorizedAccessException();
            }
            string? email = getEmail();
            // if not found email
            if(email == null)
            {
                throw new ApplicationException();
            }
            return await service.ChangePassword(DTO, email);
        }
    }
}
