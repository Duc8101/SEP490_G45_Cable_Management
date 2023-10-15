using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly OtherMaterialsService service = new OtherMaterialsService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int page = 1)
        {
            // if admin
            if(isAdmin())
            {
                return await service.List(filter, null, page);
            }
            // if warehouse keeper
            if(isWarehouseKeeper())
            {
                string? WareHouseKeeperID = getUserID();
                if(WareHouseKeeperID == null)
                {
                    return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int) HttpStatusCode.NotFound);
                }
                return await service.List(filter, Guid.Parse(WareHouseKeeperID), page);
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin, warehouse keeper
            if(isAdmin() || isWarehouseKeeper())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
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
        public async Task<ResponseDTO<bool>> Delete(int OtherMaterialsID)
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
