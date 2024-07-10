using API.Attributes;
using API.Services.NodeMaterialCategories;
using Common.Base;
using Common.Const;
using Common.DTO.NodeMaterialCategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Role(RoleConst.Admin)]
    public class NodeMaterialCategoryController : BaseAPIController
    {
        private readonly INodeMaterialCategoryService _service;

        public NodeMaterialCategoryController(INodeMaterialCategoryService service)
        {
            _service = service;
        }

        [HttpPut("{nodeId}")]     
        public ResponseBase Update([Required] Guid nodeId, [Required] NodeMaterialCategoryUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(nodeId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
