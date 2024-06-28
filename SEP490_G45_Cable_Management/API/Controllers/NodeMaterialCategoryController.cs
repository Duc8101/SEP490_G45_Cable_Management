using API.Attributes;
using API.Services.NodeMaterialCategories;
using Common.Base;
using Common.DTO.NodeMaterialCategoryDTO;
using Common.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NodeMaterialCategoryController : BaseAPIController
    {
        private readonly INodeMaterialCategoryService _service;

        public NodeMaterialCategoryController(INodeMaterialCategoryService service)
        {
            _service = service;
        }

        [HttpPut("{NodeID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] Guid NodeID, [Required] NodeMaterialCategoryUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(NodeID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
