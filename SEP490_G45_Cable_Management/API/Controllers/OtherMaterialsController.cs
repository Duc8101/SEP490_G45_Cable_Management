using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
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
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?> response;
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
                    response = new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
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
        public async Task<ResponseDTO<List<OtherMaterialsListDTO>?>> List(int? WareHouseID)
        {
            ResponseDTO<List<OtherMaterialsListDTO>?> response = await _service.ListAll(WareHouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Create([Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Create(DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{OtherMaterialsID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Update([Required] int OtherMaterialsID, [Required] OtherMaterialsCreateUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(OtherMaterialsID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{OtherMaterialsID}")]
        [Role(DataAccess.Enum.Role.Admin)]
        public async Task<ResponseDTO<bool>> Delete([Required] int OtherMaterialsID)
        {
            ResponseDTO<bool> response = await _service.Delete(OtherMaterialsID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
