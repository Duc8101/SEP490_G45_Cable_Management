using API.Attributes;
using API.Services.Routes;
using Common.Base;
using Common.DTO.RouteDTO;
using Common.Enum;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseBase List(string? name)
        {
            ResponseBase response = _service.ListAll(name);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin)]
        public ResponseBase List([Required] int page = 1)
        {
            ResponseBase response = _service.ListPaged(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public ResponseBase Create([Required] RouteCreateDTO DTO)
        {
            ResponseBase response = _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{routeId}")]
        [Role(Role.Admin)]
        public ResponseBase Delete([Required] Guid routeId)
        {
            ResponseBase response = _service.Delete(routeId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
