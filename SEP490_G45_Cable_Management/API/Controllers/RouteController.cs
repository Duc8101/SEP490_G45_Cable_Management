using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RouteController : BaseAPIController
    {
        private readonly RouteService service = new RouteService();

        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<RouteListDTO>?>> List(string? name)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.ListAll(name);
            }
            return new ResponseDTO<List<RouteListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<RouteListDTO>?>> List([Required] int page = 1)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.ListPaged(page);
            }
            return new ResponseDTO<PagedResultDTO<RouteListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public ResponseDTO<bool> Create([Required] RouteCreateDTO DTO)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{RouteID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid RouteID)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.Delete(RouteID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
