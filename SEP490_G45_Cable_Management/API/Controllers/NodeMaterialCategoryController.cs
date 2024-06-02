using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
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
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] Guid NodeID, [Required] NodeMaterialCategoryUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(NodeID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
