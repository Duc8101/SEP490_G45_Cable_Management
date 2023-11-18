using API.Services.IService;
using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.TransactionDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TransactionController : BaseAPIController
    {
        private readonly ITransactionService service;

        public TransactionController(ITransactionService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.List(filter, WareHouseID, page);
            }
            return new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{TransactionID}")]
        [Authorize]
        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail([Required] Guid TransactionID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Detail(TransactionID);
            }
            return new ResponseDTO<TransactionDetailDTO?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
