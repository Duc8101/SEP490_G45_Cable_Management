﻿using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CableController : BaseAPIController
    {
        private readonly ICableService service;

        public CableController(ICableService service)
        {
            this.service = service;
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<CableListDTO>?>> List(string? filter, int? WarehouseID, [Required] bool isExportedToUse = false, [Required] int page = 1)
        {
            // if admin, warehouse keeper, leader
            if (isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.ListPaged(filter, WarehouseID, isExportedToUse, page);
            }
            return new ResponseDTO<PagedResultDTO<CableListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<CableListDTO>?>> List(int? WarehouseID)
        {
            return await service.ListAll(WarehouseID);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] CableCreateUpdateDTO DTO)
        {
            /// if admin
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

        [HttpPut("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid CableID, [Required] CableCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(CableID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid CableID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(CableID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
