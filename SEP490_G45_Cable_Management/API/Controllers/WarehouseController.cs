using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.WarehouseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WarehouseController : BaseAPIController
    {
        private readonly WarehouseService service = new WarehouseService();

        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<WarehouseListDTO>> List(string? name , int page)
        {
            // if admin, warehousekeeper, leader
            if(isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.List(name, page);
            }
            throw new UnauthorizedAccessException();
        }

    }
}
