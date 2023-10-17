using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class IssueController : BaseAPIController
    {
        private readonly IssueService service = new IssueService();
        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List(string? filter, int page = 1)
        {
            // if admin, leader, staff
            if(isAdmin() || isLeader() || isStaff())
            {
                return await service.ListAll(filter, page);
            }
            return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet("Doing")]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List(int page = 1)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.ListDoing(page);
            }
            return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    throw new ApplicationException();
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.Update(IssueID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid IssueID)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.Delete(IssueID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
