using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly OtherMaterialsService service = new OtherMaterialsService();
        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<OtherMaterialsListDTO>> List(string? filter, int page)
        {
            // if admin, warehouse keeper , leader
            if(isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.List(filter, page);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
