using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.Entity;
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
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<CableCategoryListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseDTO<List<CableCategoryListDTO>?>> List()
        {
            ResponseDTO<List<CableCategoryListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{CableCategoryID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] int CableCategoryID, [Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(CableCategoryID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
