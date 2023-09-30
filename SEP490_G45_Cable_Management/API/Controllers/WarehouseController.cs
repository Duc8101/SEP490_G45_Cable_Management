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

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    throw new ApplicationException();
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(WarehouseID, DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{WarehouseID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(int WarehouseID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(WarehouseID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
