using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RouteController : BaseAPIController
    {
        private readonly RouteService service = new RouteService();

        [HttpGet]
        [Authorize]
        public async Task<List<RouteListDTO>> List(string? name)
        {
            // if guest
            if (isGuest())
            {
                throw new UnauthorizedAccessException();
            }
            return await service.List(name);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(RouteCreateDTO DTO)
        {
            // if admin
            if(isAdmin())
            {
                return await service.Create(DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{RouteID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid RouteID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(RouteID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
