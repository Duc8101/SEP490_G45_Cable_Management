using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RequestController : BaseAPIController
    {
        private readonly RequestService service = new RequestService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name , string? status, int page = 1)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.List(name, status, null, page);
            }
            // if warehouse keeper, staff
            if (isWarehouseKeeper() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int) HttpStatusCode.NotFound);
                }
                return await service.List(name, status, Guid.Parse(CreatorID), page);
            }
            return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        /*
        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> CreateExport(RequestCreateExportDTO DTO)
        {
            // if admin or staff
            if (isAdmin() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    throw new ApplicationException();
                }
                return await service.CreateExport(DTO, Guid.Parse(CreatorID));
            }
            throw new UnauthorizedAccessException();
        }*/
    }
}
