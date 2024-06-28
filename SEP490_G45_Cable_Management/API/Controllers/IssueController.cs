using API.Attributes;
using API.Services.Issues;
using Common.Base;
using Common.DTO.IssueDTO;
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
    public class IssueController : BaseAPIController
    {
        private readonly IIssueService _service;

        public IssueController(IIssueService service)
        {
            _service = service;
        }


        [HttpGet("Paged/All")]
        public async Task<ResponseBase<Pagination<IssueListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            ResponseBase<Pagination<IssueListDTO>?> response = await _service.ListPagedAll(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged/Doing")]
        public async Task<ResponseBase<Pagination<IssueListDTO>?>> List([Required] int page = 1)
        {
            ResponseBase<Pagination<IssueListDTO>?> response = await _service.ListPagedDoing(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Doing")]
        public async Task<ResponseBase<List<IssueListDTO>?>> List()
        {
            ResponseBase<List<IssueListDTO>?> response = await _service.ListDoing();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(Role.Admin, Role.Staff)]
        public async Task<ResponseBase<bool>> Create(IssueCreateDTO DTO)
        {
            ResponseBase<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseBase<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{IssueID}")]
        [Role(Role.Admin, Role.Staff)]
        public async Task<ResponseBase<bool>> Update([Required] Guid IssueID, [Required] IssueUpdateDTO DTO)
        {
            ResponseBase<bool> response = await _service.Update(IssueID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{IssueID}")]
        [Role(Role.Admin, Role.Staff)]
        public async Task<ResponseBase<bool>> Delete([Required] Guid IssueID)
        {
            ResponseBase<bool> response = await _service.Delete(IssueID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{IssueID}")]
        public async Task<ResponseBase<List<IssueDetailDTO>?>> Detail([Required] Guid IssueID)
        {
            ResponseBase<List<IssueDetailDTO>?> response = await _service.Detail(IssueID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
