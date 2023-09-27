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
        public async Task<ResponseDTO<bool>> Register(RegisterDTO DTO)
        {
            try
            {
                // get password for new user
                string newPw = UserUtil.RandomPassword();
                string hashPw = UserUtil.HashPassword(newPw);
                // get body email
                string body = UserUtil.BodyEmailForRegister(newPw);
                // send email
                await UserUtil.sendEmail("Welcome to Cable Management System", body, DTO.Email);
                // register
                return await service.Register(DTO, hashPw);
            }catch(Exception ex)
            {
                // if send email failed, throw message
                throw new Exception(ex.Message);
            }
        }

        
    }
}
