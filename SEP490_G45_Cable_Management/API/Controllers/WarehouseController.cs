using API.Attributes;
using API.Services.Warehouses;
using Common.Base;
using Common.DTO.WarehouseDTO;
using Common.Entity;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WarehouseController : BaseAPIController
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }


        [HttpGet("Paged")]
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase List(string? name, [Required] int page = 1)
        {
            ResponseBase response = _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public ResponseBase List()
        {
            ResponseBase response = _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Roles.Admin)]
        public ResponseBase Create([Required] WarehouseCreateUpdateDTO DTO)
        {
            Guid? creatorId = getUserId();
            ResponseBase response;
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.Create(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{warehouseId}")]
        [Role(Roles.Admin)]
        public ResponseBase Update([Required] int warehouseId, [Required] WarehouseCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(warehouseId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{warehouseId}")]
        [Role(Roles.Admin)]
        public ResponseBase Delete([Required] int warehouseId)
        {
            ResponseBase response = _service.Delete(warehouseId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
