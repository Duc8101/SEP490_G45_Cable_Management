using API.Services;
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
    }
}
