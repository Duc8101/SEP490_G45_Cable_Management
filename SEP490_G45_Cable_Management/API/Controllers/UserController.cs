using API.Services;
using DataAccess.DTO.UserDTO;
using DataAccess.Model.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost]
        public async Task<ResponseDTO<bool>> Create(RegisterDTO DTO)
        {
            return await service.Create(DTO);
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
