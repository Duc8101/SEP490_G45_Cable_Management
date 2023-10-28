using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using DataAccess.DTO.RequestDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name , string? status, [Required] int page = 1)
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
                    return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.List(name, status, Guid.Parse(CreatorID), page);
            }
            return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost("Export")]
        //[Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateExportDTO DTO)
        {
            return await service.CreateRequestExport(DTO, Guid.Parse("8CA167ED-BCB7-45EB-94F8-02188B0C3CC5"));
/*            // if warehouse keeper or staff
            if (isWarehouseKeeper() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                }
                return await service.CreateRequestExport(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
*/        }

        [HttpPost("Recovery")]
        //[Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateRecoveryDTO DTO)
        {
            /*            // if warehouse keeper or staff
                        if (isWarehouseKeeper() || isStaff())
                        {
                            string? CreatorID = getUserID();
                            if (CreatorID == null)
                            {
                                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                            }
                            return await service.CreateRequestRecovery(DTO, Guid.Parse(CreatorID));
                        }
                        return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
            */
            return await service.CreateRequestRecovery(DTO, Guid.Parse("8CA167ED-BCB7-45EB-94F8-02188B0C3CC5"));
        }

        [HttpPost("Deliver")]
        //[Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateDeliverDTO DTO)
        {
            return await service.CreateRequestDeliver(DTO, Guid.Parse("8CA167ED-BCB7-45EB-94F8-02188B0C3CC5"));
            /*            if (isWarehouseKeeper() || isStaff())
                        {
                            string? CreatorID = getUserID();
                            if (CreatorID == null)
                            {
                                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                            }
                            return await service.CreateRequestDeliver(DTO,Guid.Parse(CreatorID));
                        }
                        return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
            */
        }

        [HttpPut("{RequestID}")]
        //[Authorize]
        public async Task<ResponseDTO<bool>> Approve([Required] Guid RequestID)
        {
            return await service.Approve(RequestID, Guid.Parse("B86006A3-75D7-4AAB-A923-F26349644DDF"), "Phạm Minh Hiếu");
            /*            // if admin
                        if (isAdmin())
                        {
                            string? ApproverID = getUserID();
                            string? FirstName = getFirstName();
                            string? LastName = getLastName();
                            if (ApproverID == null)
                            {
                                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                            }
                            if (FirstName == null || LastName == null)
                            {
                                return new ResponseDTO<bool>(false, "Không tìm thấy tên của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                            }
                            return await service.Approve(RequestID, Guid.Parse(ApproverID), LastName + " " + FirstName);
                        }
                        return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
            */
        }

        [HttpPut("{RequestID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Reject([Required] Guid RequestID)
        {
            // if admin
            if (isAdmin())
            {
                string? RejectorID = getUserID();
                if (RejectorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                }
                return await service.Reject(RequestID, Guid.Parse(RejectorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }
    }
}
