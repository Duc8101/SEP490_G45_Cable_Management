using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
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
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }


        [HttpPost]
        public async Task<ResponseDTO<TokenDTO?>> Login([Required] LoginDTO DTO)
        {
            return await service.Login(DTO);
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<UserListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            // if admin
            if (isAdmin())
            {
                return await service.ListPaged(filter, page);
            }
            return new ResponseDTO<PagedResultDTO<UserListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("WarehouseKeeper")]
        [Authorize]
        public async Task<ResponseDTO<List<UserListDTO>?>> List()
        {
            if (isAdmin())
            {
                return await service.ListWarehouseKeeper();
            }
            return new ResponseDTO<List<UserListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] UserCreateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{UserID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid UserID, [Required] UserUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(UserID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{UserID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid UserID)
        {
            // if admin
            if (isAdmin())
            {
                string? UserLoginID = getUserID();
                // if not found
                if (UserLoginID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID", (int)HttpStatusCode.NotFound);
                }
                return await service.Delete(UserID, Guid.Parse(UserLoginID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public async Task<ResponseDTO<bool>> ForgotPassword([Required] ForgotPasswordDTO DTO)
        {
            return await service.ForgotPassword(DTO);
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
            return await service.ChangePassword(DTO, email);
        }
    }
}
