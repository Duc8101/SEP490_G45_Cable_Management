using API.Attributes;
using API.Services.Warehouses;
using Common.Base;
using Common.DTO.WarehouseDTO;
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
    public class WarehouseController : BaseAPIController
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }


        [HttpGet("Paged")]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<Pagination<WarehouseListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseBase<Pagination<WarehouseListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<WarehouseListDTO>?>> List()
        {
            ResponseBase<List<WarehouseListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] WarehouseCreateUpdateDTO DTO)
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

        [HttpPut("{WarehouseID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] int WarehouseID, [Required] WarehouseCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(WarehouseID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{WarehouseID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] int WarehouseID)
        {
            ResponseBase<bool> response = await _service.Delete(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
