using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.WarehouseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WarehouseController : BaseAPIController
    {
        private readonly IWarehouseService service;

        public WarehouseController(IWarehouseService service)
        {
            this.service = service;
        }


        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<WarehouseListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin, warehousekeeper, leader
            if (isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseDTO<PagedResultDTO<WarehouseListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<WarehouseListDTO>?>> List()
        {
            return await service.ListAll();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] int WarehouseID, [Required] WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(WarehouseID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] int WarehouseID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(WarehouseID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
