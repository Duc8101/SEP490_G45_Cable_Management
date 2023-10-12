using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SupplierController : BaseAPIController
    {
        private readonly SupplierService service = new SupplierService();

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> List(string? name, int page = 1 /* current page */)
        {
            // if admin or warehouse or leader
            if (isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.List(name, page);
            }
            return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO)
        { 
            // if admin
            if (isAdmin())
            {
                // get user id
                string? CreatorID = getUserID();
                // if not found user id
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra lại thông tin đăng nhập", (int) HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, CreatorID);             
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(SupplierID, DTO);  
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(SupplierID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }
    }
}
