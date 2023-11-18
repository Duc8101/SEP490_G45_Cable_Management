using API.Services.IService;
using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsCategoryController : BaseAPIController
    {
        private readonly IOtherMaterialsCategoryService service;

        public OtherMaterialsCategoryController(IOtherMaterialsCategoryService service)
        {
            this.service = service;
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<OtherMaterialsCategoryListDTO>?>> List()
        {
            return await service.ListAll();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{OtherMaterialsCategoryID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] int OtherMaterialsCategoryID, [Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Update(OtherMaterialsCategoryID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
