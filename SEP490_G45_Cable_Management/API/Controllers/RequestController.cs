using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RequestController : BaseAPIController
    {
        private readonly RequestService service = new RequestService();
        [HttpGet]
        //[Authorize]
        public async Task<PagedResultDTO<RequestListDTO>> List(string? name , string? status , int page)
        {
            // if guest
            if (isGuest())
            {
                throw new UnauthorizedAccessException();
            }
            return await service.List(name, status, page);
        }
    }
}
