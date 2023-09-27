using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async Task<PagedResultDTO<CableCategoryListDTO>> List(string? name, int page)
        {
            // if admin
            if (isAdmin())
            {
                return await service.List(name, page);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(CableCategoryCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{CableCategoryID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(CableCategoryID, DTO);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
