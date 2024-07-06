using API.Attributes;
using API.Services.OtherMaterials;
using Common.Base;
using Common.DTO.OtherMaterialsDTO;
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
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly IOtherMaterialsService _service;

        public OtherMaterialsController(IOtherMaterialsService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            ResponseBase response;
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                response = _service.ListPaged(filter, WareHouseID, null, page);
            }
            else
            {
                Guid? wareHouseKeeperId = getUserId();
                if (wareHouseKeeperId == null)
                {
                    response = new ResponseBase("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                else
                {
                    response = _service.ListPaged(filter, WareHouseID, wareHouseKeeperId.Value, page);
                }
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public ResponseBase List(int? wareHouseId)
        {
            ResponseBase response = _service.ListAll(wareHouseId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Roles.Admin)]
        public ResponseBase Create([Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{otherMaterialsId}")]
        [Role(Roles.Admin)]
        public ResponseBase Update([Required] int otherMaterialsId, [Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(otherMaterialsId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{otherMaterialsId}")]
        [Role(Roles.Admin)]
        public ResponseBase Delete([Required] int otherMaterialsId)
        {
            ResponseBase response = _service.Delete(otherMaterialsId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
