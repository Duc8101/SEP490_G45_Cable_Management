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
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.List(RouteID);
            }
            return new ResponseDTO<List<NodeListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] NodeCreateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<NodeListDTO?>> Detail([Required] Guid NodeID)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Detail(NodeID);
            }
            return new ResponseDTO<NodeListDTO?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid NodeID, [Required] NodeUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Update(NodeID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid NodeID)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Delete(NodeID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
