using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CableCategoryController : BaseAPIController
    {
        private readonly CableCategoryService service = new CableCategoryService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>> List(string? name, int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.List(name, page);
            }
            return new ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(CableCategoryCreateUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{CableCategoryID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Update(CableCategoryID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
