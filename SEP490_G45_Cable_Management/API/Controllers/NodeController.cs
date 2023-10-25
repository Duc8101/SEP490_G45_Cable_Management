using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.NodeDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NodeController : BaseAPIController
    {
        private readonly NodeService service = new NodeService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<NodeListDTO>?>> List(Guid RouteID)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.List(RouteID);
            }
            return new ResponseDTO<List<NodeListDTO>?>(null , "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(NodeCreateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid NodeID, NodeUpdateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Update(NodeID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid NodeID)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Delete(NodeID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }
    }
}
