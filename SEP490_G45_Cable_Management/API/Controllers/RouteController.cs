using API.Attributes;
using API.Services.Routes;
using Common.Base;
using Common.DTO.RouteDTO;
using Common.Enum;
using Common.Pagination;
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
        public async Task<ResponseBase<List<RouteListDTO>?>> List(string? name)
        {
            ResponseBase<List<RouteListDTO>?> response = await _service.ListAll(name);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<Pagination<RouteListDTO>?>> List([Required] int page = 1)
        {
            ResponseBase<Pagination<RouteListDTO>?> response = await _service.ListPaged(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] RouteCreateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{RouteID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] Guid RouteID)
        {
            ResponseBase<bool> response = await _service.Delete(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
