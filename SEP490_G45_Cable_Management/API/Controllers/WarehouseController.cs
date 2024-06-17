using API.Services.IService;
using Common.Base;
using Common.DTO.WarehouseDTO;
using Common.Pagination;
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
        public async Task<ResponseBase<Pagination<WarehouseListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin, warehousekeeper, leader
            if (isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseBase<Pagination<WarehouseListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseBase<List<WarehouseListDTO>?>> List()
        {
            return await service.ListAll();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseBase<bool>> Create([Required] WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                Guid? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, CreatorID.Value);
            }
            return new ResponseBase<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseBase<bool>> Update([Required] int WarehouseID, [Required] WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(WarehouseID, DTO);
            }
            return new ResponseBase<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseBase<bool>> Delete([Required] int WarehouseID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(WarehouseID);
            }
            return new ResponseBase<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
