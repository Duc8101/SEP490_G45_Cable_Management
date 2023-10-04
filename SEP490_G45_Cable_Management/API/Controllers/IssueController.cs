using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Encodings;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class IssueController : BaseAPIController
    {
        private readonly IssueService service = new IssueService();
        [HttpGet]
        //[Authorize]
        public async Task<PagedResultDTO<IssueListDTO>> List(string? filter, int page)
        {
            // if guest
            //if(isGuest())
            //{
                //throw new UnauthorizedAccessException();  
            //}
            return await service.List(filter, page);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO)
        {
            // if admin or staff
            if(isAdmin() || isStaff())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    throw new ApplicationException();
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            // if admin or staff
            if (isAdmin() || isStaff())
            {
                return await service.Update(IssueID, DTO);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(Guid IssueID)
        {
            // if admin or staff
            if (isAdmin() || isStaff())
            {
                return await service.Delete(IssueID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
