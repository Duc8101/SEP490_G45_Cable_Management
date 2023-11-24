using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NodeController : BaseAPIController
    {
        private readonly INodeService service;

        public NodeController(INodeService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<NodeListDTO>?>> List([Required] Guid RouteID)
        {
            return await service.List(RouteID);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] NodeCreateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<NodeListDTO?>> Detail([Required] Guid NodeID)
        {
            return await service.Detail(NodeID);
        }

        [HttpPut("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid NodeID, [Required] NodeUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(NodeID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid NodeID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(NodeID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
