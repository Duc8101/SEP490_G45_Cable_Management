using API.Attributes;
using API.Services.Users;
using Common.Base;
using Common.DTO.UserDTO;
using Common.Enum;
using Common.Pagination;
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
        public async Task<ResponseBase<TokenDTO?>> Login([Required] LoginDTO DTO)
        {
            ResponseBase<TokenDTO?> response = await _service.Login(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Authorize]
        [Role(Role.Admin)]
        public async Task<ResponseBase<Pagination<UserListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            ResponseBase<Pagination<UserListDTO>?> response = await _service.ListPaged(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("WarehouseKeeper")]
        [Authorize]
        [Role(Role.Admin)]
        public async Task<ResponseBase<List<UserListDTO>?>> List()
        {
            ResponseBase<List<UserListDTO>?> response = await _service.ListWarehouseKeeper();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Authorize]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] UserCreateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{UserID}")]
        [Authorize]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] Guid UserID, [Required] UserUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(UserID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{UserID}")]
        [Authorize]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] Guid UserID)
        {
            ResponseBase<bool> response;
            Guid? UserLoginID = getUserID();
            // if not found
            if (UserLoginID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Delete(UserID, UserLoginID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        public async Task<ResponseBase<bool>> ForgotPassword([Required] ForgotPasswordDTO DTO)
        {
            ResponseBase<bool> response = await _service.ForgotPassword(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseBase<bool>> ChangePassword([Required] ChangePasswordDTO DTO)
        {
            ResponseBase<bool> response;
            string? email = getEmail();
            // if not found email
            if (email == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy email. Cần xác minh lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.ChangePassword(DTO, email);
            }
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
