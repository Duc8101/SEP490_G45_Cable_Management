using API.Attributes;
using API.Services.Suppliers;
using Common.Base;
using Common.DTO.SupplierDTO;
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
    public class SupplierController : BaseAPIController
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
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
        public ResponseBase Create([Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseBase response;
            // get user id
            Guid? creatorId = getUserId();
            // if not found user id
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra lại thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.Create(DTO, creatorId.Value);
            }
            return response;
        }

        [HttpPut("{supplierId}")]
        [Role(Roles.Admin)]
        public ResponseBase Update([Required] int supplierId, [Required] SupplierCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(supplierId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{supplierId}")]
        [Role(Roles.Admin)]
        public ResponseBase Delete([Required] int supplierId)
        {
            ResponseBase response = _service.Delete(supplierId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
