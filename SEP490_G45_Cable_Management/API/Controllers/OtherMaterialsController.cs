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
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int page)
        {
            // if admin, warehouse keeper , leader
            if(isAdmin() || isWarehouseKeeper() || isLeader())
            {
                return await service.List(filter, page);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin
            if(isAdmin())
            {
                return await service.Create(DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(OtherMaterialsID, DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{OtherMaterialsID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(int OtherMaterialsID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(OtherMaterialsID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
