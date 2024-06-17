using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.NodeDTO;
using Common.Enum;
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
        public async Task<ResponseBase<List<NodeListDTO>?>> List([Required] Guid RouteID)
        {
            ResponseBase<List<NodeListDTO>?> response = await _service.List(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] NodeCreateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{NodeID}")]
        public async Task<ResponseBase<NodeListDTO?>> Detail([Required] Guid NodeID)
        {
            ResponseBase<NodeListDTO?> response = await _service.Detail(NodeID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{NodeID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] Guid NodeID, [Required] NodeUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(NodeID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{NodeID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] Guid NodeID)
        {
            ResponseBase<bool> response = await _service.Delete(NodeID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
