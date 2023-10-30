using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using API.Services.Service;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsCategoryController : BaseAPIController
    {
        private readonly OtherMaterialsCategoryService service = new OtherMaterialsCategoryService();
        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<OtherMaterialsCategoryListDTO>?>> List()
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.ListAll();
            }
            return new ResponseDTO<List<OtherMaterialsCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
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
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
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
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }
    }
}
