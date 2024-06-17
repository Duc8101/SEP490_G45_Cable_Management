using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.OtherMaterialsDTO;
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
    public class OtherMaterialsController : BaseAPIController
    {
        private readonly IOtherMaterialsService _service;

        public OtherMaterialsController(IOtherMaterialsService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<Pagination<OtherMaterialsListDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            ResponseBase<Pagination<OtherMaterialsListDTO>?> response;
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                response = await _service.ListPaged(filter, WareHouseID, null, page);
            }
            else
            {
                Guid? WareHouseKeeperID = getUserID();
                if (WareHouseKeeperID == null)
                {
                    response = new ResponseBase<Pagination<OtherMaterialsListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                else
                {
                    response = await _service.ListPaged(filter, WareHouseID, WareHouseKeeperID.Value, page);
                }
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("All")]
        public async Task<ResponseBase<List<OtherMaterialsListDTO>?>> List(int? WareHouseID)
        {
            ResponseBase<List<OtherMaterialsListDTO>?> response = await _service.ListAll(WareHouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Create([Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{OtherMaterialsID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Update([Required] int OtherMaterialsID, [Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(OtherMaterialsID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{OtherMaterialsID}")]
        [Role(Role.Admin)]
        public async Task<ResponseBase<bool>> Delete([Required] int OtherMaterialsID)
        {
            ResponseBase<bool> response = await _service.Delete(OtherMaterialsID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
