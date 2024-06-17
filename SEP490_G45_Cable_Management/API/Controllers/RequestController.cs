using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.RequestDTO;
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
    public class RequestController : BaseAPIController
    {
        private readonly IRequestService _service;

        public RequestController(IRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ResponseBase<Pagination<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, [Required] int page = 1)
        {
            ResponseBase<Pagination<RequestListDTO>?> response;
            // if admin, leader
            if (isAdmin() || isLeader())
            {
                response = await _service.List(name, RequestCategoryID, status, null, page);
            }
            else
            {
                Guid? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    response = new ResponseBase<Pagination<RequestListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                else
                {
                    response = await _service.List(name, RequestCategoryID, status, CreatorID.Value, page);
                }
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Export")]
        public async Task<ResponseBase<bool>> Create([Required] RequestCreateExportDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestExport(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Recovery")]
        public async Task<ResponseBase<bool>> Create([Required] RequestCreateRecoveryDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestRecovery(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Deliver")]
        public async Task<ResponseBase<bool>> Create([Required] RequestCreateDeliverDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestDeliver(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{RequestID}")]
        [Role(Role.Admin, Role.Leader)]
        public async Task<ResponseBase<bool>> Approve([Required] Guid RequestID)
        {
            ResponseBase<bool> response;
            Guid? ApproverID = getUserID();
            string? FirstName = getFirstName();
            string? LastName = getLastName();
            if (ApproverID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else if (FirstName == null || LastName == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy tên của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.Approve(RequestID, ApproverID.Value, LastName + " " + FirstName);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{RequestID}")]
        [Role(Role.Admin, Role.Leader)]
        public async Task<ResponseBase<bool>> Reject([Required] Guid RequestID)
        {
            ResponseBase<bool> response;
            Guid? RejectorID = getUserID();
            if (RejectorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.Reject(RequestID, RejectorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Inside-Warehouse")]
        [Role(Role.Admin, Role.Staff, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<bool>> Create([Required] RequestCreateCancelInsideDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelInside(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Outside-Warehouse")]
        [Role(Role.Admin, Role.Staff, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<bool>> Create([Required] RequestCreateCancelOutsideDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelOutside(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{RequestID}")]
        public async Task<ResponseBase<bool>> Delete([Required] Guid RequestID)
        {
            ResponseBase<bool> response = await _service.Delete(RequestID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{RequestID}")]
        public async Task<ResponseBase<RequestDetailDTO?>> Detail([Required] Guid RequestID)
        {
            ResponseBase<RequestDetailDTO?> response = await _service.Detail(RequestID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
