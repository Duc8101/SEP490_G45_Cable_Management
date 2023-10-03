using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RequestController : BaseAPIController
    {
        private readonly RequestService service = new RequestService();
        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<RequestListDTO>> List(string? name , string? status , int page)
        {
            // if guest
            if (isGuest())
            {
                throw new UnauthorizedAccessException();
            }
            return await service.List(name, status, page);
        }
        /*
        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(RequestCreateDTO DTO)
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
        }*/
    }
}
