using API.Attributes;
using API.Services.Requests;
using Common.Base;
using Common.Const;
using Common.DTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseBase List(string? name, int? requestCategoryId, string? status, [Required] int page = 1)
        {
            ResponseBase response;
            // if admin, leader
            if (isWarehouseKeeper() || isStaff())
            {
                Guid? creatorId = getUserId();
                if (creatorId == null)
                {
                    response = new ResponseBase("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                else
                {
                    response = _service.List(name, requestCategoryId, status, creatorId.Value, page);
                }
            }
            else
            {
                response = _service.List(name, requestCategoryId, status, null, page);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Export")]
        public async Task<ResponseBase> Create([Required] RequestCreateExportDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestExport(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Recovery")]
        public async Task<ResponseBase> Create([Required] RequestCreateRecoveryDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestRecovery(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Deliver")]
        public async Task<ResponseBase> Create([Required] RequestCreateDeliverDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestDeliver(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{requestId}")]
        [Role(RoleConst.Admin, RoleConst.Leader)]
        public async Task<ResponseBase> Approve([Required] Guid requestId)
        {
            ResponseBase response;
            Guid? approverId = getUserId();
            string? FirstName = getFirstName();
            string? LastName = getLastName();
            if (approverId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else if (FirstName == null || LastName == null)
            {
                response = new ResponseBase(false, "Không tìm thấy tên của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.Approve(requestId, approverId.Value, LastName + " " + FirstName);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{requestId}")]
        [Role(RoleConst.Admin, RoleConst.Leader)]
        public ResponseBase Reject([Required] Guid requestId)
        {
            ResponseBase response;
            Guid? rejectorId = getUserId();
            if (rejectorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = _service.Reject(requestId, rejectorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Inside-Warehouse")]
        public async Task<ResponseBase> Create([Required] RequestCreateCancelInsideDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelInside(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Outside-Warehouse")]
        public async Task<ResponseBase> Create([Required] RequestCreateCancelOutsideDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelOutside(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{requestId}")]
        public ResponseBase Delete([Required] Guid requestId)
        {
            ResponseBase response = _service.Delete(requestId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{requestId}")]
        public ResponseBase Detail([Required] Guid requestId)
        {
            ResponseBase response = _service.Detail(requestId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
