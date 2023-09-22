using API.Services;
using DataAccess.DTO.CommonDTO;
using DataAccess.DTO.UserDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : BaseAPIController
    {
        private readonly UserService service = new UserService();

        [HttpPost]
        public async Task<BaseResponseDTO<TokenResponse>> Login([Required] LoginDTO DTO)
        {
            return await service.Login(DTO);
        }
    }
}
