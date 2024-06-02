using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CableController : BaseAPIController
    {
        private readonly ICableService _service;

        public CableController(ICableService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Warehouse_Keeper, DataAccess.Enum.Role.Leader)]
        public async Task<ResponseDTO<PagedResultDTO<CableListDTO>?>> List(string? filter, int? WarehouseID, [Required] bool isExportedToUse = false, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<CableListDTO>?> response = await _service.ListPaged(filter, WarehouseID, isExportedToUse, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseDTO<List<CableListDTO>?>> List(int? WarehouseID)
        {
            ResponseDTO<List<CableListDTO>?> response = await _service.ListAll(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] CableCreateUpdateDTO DTO)
        {
            Guid? CreatorID = getUserID();
            ResponseDTO<bool> response;
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{CableID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] Guid CableID, [Required] CableCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(CableID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{CableID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid CableID)
        {
            ResponseDTO<bool> response = await _service.Delete(CableID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
