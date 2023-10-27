using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly OtherMaterialsService service = new OtherMaterialsService();
        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            // if admin
            if(isAdmin())
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
            // if warehouse keeper or staff
            if (isWarehouseKeeper() || isStaff())
            {
                return await service.ListAll(WareHouseID);
            }
            return new ResponseDTO<List<OtherMaterialsListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin, warehouse keeper
            if(isAdmin() || isWarehouseKeeper())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] int OtherMaterialsID, [Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin, warehouse keeper
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Update(OtherMaterialsID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] int OtherMaterialsID)
        {
            // if admin, warehouse keeper
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Delete(OtherMaterialsID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }
    }
}
