using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
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
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> List(string? name, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<SupplierListDTO>?> response = await _service.ListPaged(name, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseDTO<List<SupplierListDTO>?>> List()
        {
            ResponseDTO<List<SupplierListDTO>?> response = await _service.ListAll();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response;
            // get user id
            Guid? CreatorID = getUserID();
            // if not found user id
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            return response;
        }

        [HttpPut("{SupplierID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] int SupplierID, [Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(SupplierID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{SupplierID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] int SupplierID)
        {
            ResponseDTO<bool> response = await _service.Delete(SupplierID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
