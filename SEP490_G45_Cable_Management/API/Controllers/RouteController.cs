using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RouteController : BaseAPIController
    {
        private readonly IRouteService _service;

        public RouteController(IRouteService service)
        {
            _service = service;
        }


        [HttpGet("All")]
        public async Task<ResponseDTO<List<RouteListDTO>?>> List(string? name)
        {
            ResponseDTO<List<RouteListDTO>?> response = await _service.ListAll(name);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<PagedResultDTO<RouteListDTO>?>> List([Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<RouteListDTO>?> response = await _service.ListPaged(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] RouteCreateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{RouteID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid RouteID)
        {
            ResponseDTO<bool> response = await _service.Delete(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
