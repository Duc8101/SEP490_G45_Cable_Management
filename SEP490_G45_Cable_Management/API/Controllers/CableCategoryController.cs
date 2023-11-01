using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.DTO.UserDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CableCategoryController : BaseAPIController
    {
        private readonly CableCategoryService service = new CableCategoryService();
        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.ListPaged(name, page);
            }
            return new ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<CableCategoryListDTO>?>> List()
        {
            return await service.ListAll();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] CableCategoryCreateUpdateDTO DTO)
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
        public async Task<ResponseDTO<bool>> Update([Required] int CableCategoryID, [Required] CableCategoryCreateUpdateDTO DTO)
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
