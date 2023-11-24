using API.Services.IService;
using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;
using DataAccess.DTO.RequestDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RequestController : BaseAPIController
    {
        private readonly IRequestService service;

        public RequestController(IRequestService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, [Required] int page = 1)
        {
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                return await service.List(name, RequestCategoryID, status, null, page);
            }
            // if warehouse keeper, staff
            if (isWarehouseKeeper() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.List(name, RequestCategoryID, status, Guid.Parse(CreatorID), page);
            }
            return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost("Export")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateExportDTO DTO)
        {
            string? CreatorID = getUserID();
            if (CreatorID == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            return await service.CreateRequestExport(DTO, Guid.Parse(CreatorID));
        }

        [HttpPost("Recovery")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateRecoveryDTO DTO)
        {
            string? CreatorID = getUserID();
            if (CreatorID == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            return await service.CreateRequestRecovery(DTO, Guid.Parse(CreatorID));
        }

        [HttpPost("Deliver")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateDeliverDTO DTO)
        {
            string? CreatorID = getUserID();
            if (CreatorID == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            return await service.CreateRequestDeliver(DTO, Guid.Parse(CreatorID));
        }

        [HttpPut("{RequestID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Approve([Required] Guid RequestID)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
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
        }

        [HttpPut("{RequestID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Reject([Required] Guid RequestID)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                string? RejectorID = getUserID();
                if (RejectorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                }
                return await service.Reject(RequestID, Guid.Parse(RejectorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<List<CableListDTO>?>> SuggestionCable(SuggestionCableDTO suggestion)
        {
            return await service.SuggestionCable(suggestion);
        }

        [HttpPost("Cancel-Inside-Warehouse")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateCancelInsideDTO DTO)
        {
            // if admin or warehouse keeper or staff
            if (isAdmin() || isWarehouseKeeper() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                }
                return await service.CreateRequestCancelInside(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost("Cancel-Outside-Warehouse")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateCancelOutsideDTO DTO)
        {
            // if admin or warehouse keeper or staff
            if (isAdmin() || isWarehouseKeeper() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
                }
                return await service.CreateRequestCancelOutside(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{RequestID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid RequestID)
        {
            return await service.Delete(RequestID);
        }

        [HttpGet("{RequestID}")]
        [Authorize]
        public async Task<ResponseDTO<RequestDetailDTO?>> Detail([Required] Guid RequestID)
        {
            return await service.Detail(RequestID);
        }
    }
}
