using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : BaseAPIController
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<ResponseDTO<TokenDTO?>> Login([Required] LoginDTO DTO)
        {
            ResponseDTO<TokenDTO?> response = await _service.Login(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Authorize]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<PagedResultDTO<UserListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<UserListDTO>?> response = await _service.ListPaged(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("WarehouseKeeper")]
        [Authorize]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<List<UserListDTO>?>> List()
        {
            ResponseDTO<List<UserListDTO>?> response = await _service.ListWarehouseKeeper();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Authorize]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] UserCreateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{UserID}")]
        [Authorize]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] Guid UserID, [Required] UserUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(UserID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{UserID}")]
        [Authorize]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid UserID)
        {
            ResponseDTO<bool> response;
            Guid? UserLoginID = getUserID();
            // if not found
            if (UserLoginID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Delete(UserID, UserLoginID.Value);
            }
            return response;
        }

        [HttpPost]
        public async Task<ResponseDTO<bool>> ForgotPassword([Required] ForgotPasswordDTO DTO)
        {
            return await _service.ForgotPassword(DTO);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> ChangePassword([Required] ChangePasswordDTO DTO)
        {
            string? email = getEmail();
            // if not found email
            if (email == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy email. Cần xác minh lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            return await _service.ChangePassword(DTO, email);
        }
    }
}
