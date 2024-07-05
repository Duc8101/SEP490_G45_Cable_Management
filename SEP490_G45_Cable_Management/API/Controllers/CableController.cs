using API.Attributes;
using API.Services.Cables;
using Common.Base;
using Common.DTO.CableDTO;
using Common.Enum;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseBase List(string? filter, int? warehouseId, [Required] bool isExportedToUse = false, [Required] int page = 1)
        {
            ResponseBase response = _service.ListPaged(filter, warehouseId, isExportedToUse, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public ResponseBase List(int? warehouseId)
        {
            ResponseBase response = _service.ListAll(warehouseId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public ResponseBase Create([Required] CableCreateUpdateDTO DTO)
        {
            Guid? creatorId = getUserId();
            ResponseBase response;
            if (creatorId == null)
            {
                response = new ResponseBase("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.Create(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{cableId}")]
        [Role(Role.Admin)]
        public ResponseBase Update([Required] Guid cableId, [Required] CableCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(cableId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{cableId}")]
        [Role(Role.Admin)]
        public ResponseBase Delete([Required] Guid cableId)
        {
            ResponseBase response = _service.Delete(cableId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
