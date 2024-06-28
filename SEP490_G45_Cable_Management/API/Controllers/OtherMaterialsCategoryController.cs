using API.Attributes;
using API.Services.OtherMaterialsCategories;
using Common.Base;
using Common.DTO.OtherMaterialsCategoryDTO;
using Common.Enum;
using Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OtherMaterialsCategoryController : BaseAPIController
    {
        private readonly IOtherMaterialsCategoryService _service;

        public OtherMaterialsCategoryController(IOtherMaterialsCategoryService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<OtherMaterialsCategoryListDTO>?>> List()
        {
            ResponseBase<List<OtherMaterialsCategoryListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{OtherMaterialsCategoryID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] int OtherMaterialsCategoryID, [Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(OtherMaterialsCategoryID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
