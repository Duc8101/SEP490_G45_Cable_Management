using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.SupplierDTO;
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
    public class SupplierController : BaseAPIController
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<Pagination<SupplierListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseBase<Pagination<SupplierListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<SupplierListDTO>?>> List()
        {
            ResponseBase<List<SupplierListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response;
            // get user id
            Guid? CreatorID = getUserID();
            // if not found user id
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            return response;
        }

        [HttpPut("{SupplierID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] int SupplierID, [Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(SupplierID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{SupplierID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] int SupplierID)
        {
            ResponseBase<bool> response = await _service.Delete(SupplierID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
