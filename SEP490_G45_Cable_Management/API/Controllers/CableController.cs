using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CableController : BaseAPIController
    {
        private readonly CableService service = new CableService();

        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<CableListDTO>> List(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            // if admin, warehouse keeper, leader
            if(isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.List(filter, WarehouseID, isExportedToUse, page);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(CableCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    throw new ApplicationException();
                }
                return await service.Create(DTO,Guid.Parse(CreatorID));
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(CableID, DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{CableID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid CableID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(CableID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
