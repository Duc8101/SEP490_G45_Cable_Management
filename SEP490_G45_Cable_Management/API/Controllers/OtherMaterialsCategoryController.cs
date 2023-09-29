using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsCategoryController : BaseAPIController
    {
        private readonly OtherMaterialsCategoryService service = new OtherMaterialsCategoryService();
        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<OtherMaterialsCategoryListDTO>> List(int page)
        {
            // if admin
            if (isAdmin())
            {
                return await service.List(page);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{OtherMaterialsCategoryID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsCategoryID,OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(OtherMaterialsCategoryID, DTO);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
