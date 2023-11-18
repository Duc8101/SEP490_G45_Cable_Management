using API.Services.IService;
using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SupplierController : BaseAPIController
    {
        private readonly ISupplierService service;

        public SupplierController(ISupplierService service)
        {
            this.service = service;
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin or warehouse or leader
            if (isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<SupplierListDTO>?>> List()
        {
            return await service.ListAll();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] SupplierCreateUpdateDTO DTO)
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
                return await service.Create(DTO, Guid.Parse(CreatorID));             
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] int SupplierID, [Required] SupplierCreateUpdateDTO DTO)
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
        public async Task<ResponseDTO<bool>> Delete([Required] int SupplierID)
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
