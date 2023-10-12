using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsCategoryController : BaseAPIController
    {
        private readonly OtherMaterialsCategoryService service = new OtherMaterialsCategoryService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> List(string? name, int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.List(name, page);
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
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
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsCategoryID,OtherMaterialsCategoryCreateUpdateDTO DTO)
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
