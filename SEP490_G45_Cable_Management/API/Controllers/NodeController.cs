using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeDTO;
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
        public async Task<ResponseDTO<List<NodeListDTO>?>> List([Required] Guid RouteID)
        {
            ResponseDTO<List<NodeListDTO>?> response = await _service.List(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] NodeCreateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{NodeID}")]
        public async Task<ResponseDTO<NodeListDTO?>> Detail([Required] Guid NodeID)
        {
            ResponseDTO<NodeListDTO?> response = await _service.Detail(NodeID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{NodeID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] Guid NodeID, [Required] NodeUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(NodeID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{NodeID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid NodeID)
        {
            ResponseDTO<bool> response = await _service.Delete(NodeID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
