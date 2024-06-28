using API.Attributes;
using API.Services.CableCategories;
using Common.Base;
using Common.DTO.CableCategoryDTO;
using Common.Enum;
using Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CableCategoryController : BaseAPIController
    {
        private readonly ICableCategoryService _service;

        public CableCategoryController(ICableCategoryService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<Pagination<CableCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseBase<Pagination<CableCategoryListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<CableCategoryListDTO>?>> List()
        {
            ResponseBase<List<CableCategoryListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{CableCategoryID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] int CableCategoryID, [Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(CableCategoryID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
