using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class IssueController : BaseAPIController
    {
        private readonly IIssueService service;

        public IssueController(IIssueService service)
        {
            this.service = service;
        }


        [HttpGet("Paged/All")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            return await service.ListPagedAll(filter, page);
        }

        [HttpGet("Paged/Doing")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List([Required] int page = 1)
        {
            return await service.ListPagedDoing(page);
        }

        [HttpGet("Doing")]
        [Authorize]
        public async Task<ResponseDTO<List<IssueListDTO>?>> List()
        {
            return await service.ListDoing();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO)
        {
            // if admin, staff
            if (isAdmin() || isStaff())
            {
                string? CreatorID = getUserID();
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int)HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPut("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid IssueID, [Required] IssueUpdateDTO DTO)
        {
            // if admin or staff
            if (isAdmin() || isStaff())
            {
                return await service.Update(IssueID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid IssueID)
        {
            // if admin or staff
            if (isAdmin() || isStaff())
            {
                return await service.Delete(IssueID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<List<IssueDetailDTO>?>> Detail([Required] Guid IssueID)
        {
            return await service.Detail(IssueID);
        }
    }
}
