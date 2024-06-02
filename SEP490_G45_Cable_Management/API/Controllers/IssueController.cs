using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
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
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<IssueListDTO>?> response = await _service.ListPagedAll(filter, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Paged/Doing")]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List([Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<IssueListDTO>?> response = await _service.ListPagedDoing(page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("Doing")]
        public async Task<ResponseDTO<List<IssueListDTO>?>> List()
        {
            ResponseDTO<List<IssueListDTO>?> response = await _service.ListDoing();
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPost]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Staff)]
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO)
        {
            ResponseDTO<bool> response;
            Guid? CreatorID = getUserID();
            if (CreatorID == null)
            {
                response = new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
            }
            else
            {
                response = await _service.Create(DTO, CreatorID.Value);
            }
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpPut("{IssueID}")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Staff)]
        public async Task<ResponseDTO<bool>> Update([Required] Guid IssueID, [Required] IssueUpdateDTO DTO)
        {
            ResponseDTO<bool> response = await _service.Update(IssueID, DTO);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpDelete("{IssueID}")]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Staff)]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid IssueID)
        {
            ResponseDTO<bool> response = await _service.Delete(IssueID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{IssueID}")]
        public async Task<ResponseDTO<List<IssueDetailDTO>?>> Detail([Required] Guid IssueID)
        {
            ResponseDTO<List<IssueDetailDTO>?> response = await _service.Detail(IssueID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
