using API.Attributes;
using API.Services.CableCategories;
using Common.Base;
using Common.Const;
using Common.DTO.CableCategoryDTO;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseBase Create([Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{cableCategoryId}")]
        [Role(RoleConst.Admin)]
        public ResponseBase Update([Required] int cableCategoryId, [Required] CableCategoryCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(cableCategoryId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
