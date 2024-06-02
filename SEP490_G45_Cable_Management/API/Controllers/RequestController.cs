using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;
using DataAccess.Entity;
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
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<RequestListDTO>?> response;
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
                    response = new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
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
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateExportDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestExport(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Recovery")]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateRecoveryDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestRecovery(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Deliver")]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateDeliverDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestDeliver(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{RequestID}")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader)]
        public async Task<ResponseDTO<bool>> Approve([Required] Guid RequestID)
        {
            ResponseDTO<bool> response;
            Guid? ApproverID = getUserID();
            string? FirstName = getFirstName();
            string? LastName = getLastName();
            if (ApproverID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else if (FirstName == null || LastName == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy tên của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.Approve(RequestID, ApproverID.Value, LastName + " " + FirstName);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{RequestID}")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader)]
        public async Task<ResponseDTO<bool>> Reject([Required] Guid RequestID)
        {
            ResponseDTO<bool> response;
            Guid? RejectorID = getUserID();
            if (RejectorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.Reject(RequestID, RejectorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Inside-Warehouse")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Staff, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateCancelInsideDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelInside(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost("Cancel-Outside-Warehouse")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Staff, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<bool>> Create([Required] RequestCreateCancelOutsideDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập");
            }
            else
            {
                response = await _service.CreateRequestCancelOutside(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{RequestID}")]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid RequestID)
        {
            ResponseDTO<bool> response = await _service.Delete(RequestID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{RequestID}")]
        public async Task<ResponseDTO<RequestDetailDTO?>> Detail([Required] Guid RequestID)
        {
            ResponseDTO<RequestDetailDTO?> response = await _service.Detail(RequestID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
