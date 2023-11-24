using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly IOtherMaterialsService service;

        public OtherMaterialsController(IOtherMaterialsService service)
        {
            this.service = service;
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.ListPaged(filter, WareHouseID, null, page);
            }
            // if warehouse keeper
            if (isWarehouseKeeper())
            {
                string? WareHouseKeeperID = getUserID();
                if (WareHouseKeeperID == null)
                {
                    return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.ListPaged(filter, WareHouseID, Guid.Parse(WareHouseKeeperID), page);
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<OtherMaterialsListDTO>?>> List(int? WareHouseID)
        {
            return await service.ListAll(WareHouseID);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] int OtherMaterialsID, [Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(OtherMaterialsID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] int OtherMaterialsID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(OtherMaterialsID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
