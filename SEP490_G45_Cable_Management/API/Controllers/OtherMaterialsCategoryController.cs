using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
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
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseDTO<List<OtherMaterialsCategoryListDTO>?>> List()
        {
            ResponseDTO<List<OtherMaterialsCategoryListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{OtherMaterialsCategoryID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] int OtherMaterialsCategoryID, [Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(OtherMaterialsCategoryID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
