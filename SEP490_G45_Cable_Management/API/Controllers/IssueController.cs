using API.Attributes;
using API.Services.Issues;
using Common.Base;
using Common.DTO.IssueDTO;
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
    public class IssueController : BaseAPIController
    {
        private readonly IIssueService _service;

        public IssueController(IIssueService service)
        {
            _service = service;
        }


        [HttpGet("Paged/All")]
        public ResponseBase List(string? filter, [Required] int page = 1)
        {
            ResponseBase response = _service.ListPagedAll(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged/Doing")]
        public ResponseBase List([Required] int page = 1)
        {
            ResponseBase response = _service.ListPagedDoing(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Doing")]
        public ResponseBase List()
        {
            ResponseBase response = _service.ListDoing();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin, Role.Staff)]
        public ResponseBase Create(IssueCreateDTO DTO)
        {
            ResponseBase response;
            Guid? creatorId = getUserId();
            if (creatorId == null)
            {
                response = new ResponseBase(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = _service.Create(DTO, creatorId.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{issueId}")]
        [Role(Role.Admin, Role.Staff)]
        public ResponseBase Update([Required] Guid issueId, [Required] IssueUpdateDTO DTO)
        {
            ResponseBase response = _service.Update(issueId, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{issueId}")]
        [Role(Role.Admin, Role.Staff)]
        public ResponseBase Delete([Required] Guid issueId)
        {
            ResponseBase response = _service.Delete(issueId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{issueId}")]
        public ResponseBase Detail([Required] Guid issueId)
        {
            ResponseBase response = _service.Detail(issueId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
