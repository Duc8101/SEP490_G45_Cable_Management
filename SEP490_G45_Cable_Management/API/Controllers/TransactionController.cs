using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.TransactionDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TransactionController : BaseAPIController
    {
        private readonly TransactionService service = new TransactionService();
        [HttpGet]
        //[Authorize]
        public async Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page)
        {
            // if admin
            //if (isAdmin())
            //{
                return await service.List(filter, WareHouseID, page);
            //}
            //throw new UnauthorizedAccessException();
        }

        [HttpGet("{TransactionID}")]
        //[Authorize]

        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail(Guid TransactionID)
        {
            // if admin
            //if (isAdmin())
            //{
                return await service.Detail(TransactionID);
            //}
            //throw new UnauthorizedAccessException();
        }
    }
}
