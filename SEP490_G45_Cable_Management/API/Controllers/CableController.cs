using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.CableDTO;
using Common.Enum;
using Common.Pagination;
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
        [Role(Role.Admin, Role.Warehouse_Keeper, Role.Leader)]
        public async Task<ResponseBase<Pagination<CableListDTO>?>> List(string? filter, int? WarehouseID, [Required] bool isExportedToUse = false, [Required] int page = 1)
        {
            ResponseBase<Pagination<CableListDTO>?> response = await _service.ListPaged(filter, WarehouseID, isExportedToUse, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<CableListDTO>?>> List(int? WarehouseID)
        {
            ResponseBase<List<CableListDTO>?> response = await _service.ListAll(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] CableCreateUpdateDTO DTO)
        {
            Guid? CreatorID = getUserID();
            ResponseBase<bool> response;
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{CableID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] Guid CableID, [Required] CableCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(CableID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{CableID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] Guid CableID)
        {
            ResponseBase<bool> response = await _service.Delete(CableID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
