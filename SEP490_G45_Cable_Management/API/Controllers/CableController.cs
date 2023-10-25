﻿using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CableController : BaseAPIController
    {
        private readonly CableService service = new CableService();

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<CableListDTO>?>> List(string? filter, int? WarehouseID, bool isExportedToUse, int page = 1)
        {
            // if admin, warehouse keeper
            if(isAdmin() || isWarehouseKeeper())
            {
                return await service.List(filter, WarehouseID, isExportedToUse, page);
            }
            return new ResponseDTO<PagedResultDTO<CableListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(CableCreateUpdateDTO DTO)
        {
            /// if admin, warehouse keeper
            if (isAdmin() || isWarehouseKeeper())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int) HttpStatusCode.NotFound);
                }
                return await service.Create(DTO,Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO)
        {
            // if admin, warehouse keeper
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Update(CableID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid CableID)
        {
            // if admin, warehouse keeper
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Delete(CableID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }
    }
}
