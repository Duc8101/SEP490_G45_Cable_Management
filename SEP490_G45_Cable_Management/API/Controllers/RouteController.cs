using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RouteController : BaseAPIController
    {
        private readonly RouteService service = new RouteService();

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<RouteListDTO>?>> List(string? name)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.List(name);
            }
            return new ResponseDTO<List<RouteListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(RouteCreateDTO DTO)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpDelete("{RouteID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid RouteID)
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
