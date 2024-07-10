using API.Attributes;
using API.Services.Nodes;
using Common.Base;
using Common.Const;
using Common.DTO.NodeDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NodeController : BaseAPIController
    {
        private readonly INodeService _service;

        public NodeController(INodeService service)
        {
            _service = service;
        }

        [HttpGet]
        public ResponseBase List([Required] Guid routeId)
        {
            ResponseBase response = _service.List(routeId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(RoleConst.Admin)]
        public ResponseBase Create([Required] NodeCreateDTO DTO)
        {
            ResponseBase response = _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{nodeId}")]
        public ResponseBase Detail([Required] Guid nodeId)
        {
            ResponseBase response = _service.Detail(nodeId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{nodeId}")]
        [Role(RoleConst.Admin)]
        public ResponseBase Update([Required] Guid nodeId, [Required] NodeUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(nodeId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{nodeId}")]
        [Role(RoleConst.Admin)]
        public ResponseBase Delete([Required] Guid nodeId)
        {
            ResponseBase response = _service.Delete(nodeId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
