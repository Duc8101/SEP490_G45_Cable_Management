using API.Attributes;
using API.Services.OtherMaterialsCategories;
using Common.Base;
using Common.Const;
using Common.DTO.OtherMaterialsCategoryDTO;
using Microsoft.AspNetCore.Authorization;
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
        [Role(RoleConst.Admin)]
        public ResponseBase List(string? name, [Required] int page = 1)
        {
            ResponseBase response = _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public ResponseBase List()
        {
            ResponseBase response = _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(RoleConst.Admin)]
        public ResponseBase Create([Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{otherMaterialsCategoryId}")]
        [Role(RoleConst.Admin)]
        public ResponseBase Update([Required] int otherMaterialsCategoryId, [Required] OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(otherMaterialsCategoryId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
