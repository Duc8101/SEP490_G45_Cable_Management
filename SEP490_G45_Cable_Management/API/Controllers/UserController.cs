using API.Attributes;
using API.Services.Users;
using Common.Base;
using Common.DTO.UserDTO;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseBase Login([Required] LoginDTO DTO)
        {
            ResponseBase response = _service.Login(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Authorize]
        [Role(Roles.Admin)]
        public ResponseBase List(string? filter, [Required] int page = 1)
        {
            ResponseBase response = _service.ListUserPaged(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("WarehouseKeeper")]
        [Authorize]
        [Role(Roles.Admin)]
        public ResponseBase List()
        {
            ResponseBase response = _service.ListWarehouseKeeper();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Authorize]
        [Role(Roles.Admin)]
        public async Task<ResponseBase> Create([Required] UserCreateDTO DTO)
        {
            ResponseBase response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{userId}")]
        [Authorize]
        [Role(Roles.Admin)]
        public ResponseBase Update([Required] Guid userId, [Required] UserUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(userId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{userId}")]
        [Authorize]
        [Role(Roles.Admin)]
        public ResponseBase Delete([Required] Guid userId)
        {
            ResponseBase response;
            Guid? userLoginId = getUserId();
            // if not found
            if (userLoginId == null)
            {
                response = new ResponseBase("Không tìm thấy Id", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.Delete(userId, userLoginId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        public async Task<ResponseBase> ForgotPassword([Required] ForgotPasswordDTO DTO)
        {
            ResponseBase response = await _service.ForgotPassword(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut]
        [Authorize]
        public ResponseBase ChangePassword([Required] ChangePasswordDTO DTO)
        {
            ResponseBase response;
            string? email = getEmail();
            // if not found email
            if (email == null)
            {
                response = new ResponseBase(false, "Không tìm thấy email. Cần xác minh lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.ChangePassword(DTO, email);
            }
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
