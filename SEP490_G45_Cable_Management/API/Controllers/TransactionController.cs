using API.Attributes;
using API.Services.Transaction;
using Common.Base;
using Common.DTO.TransactionDTO;
using Common.Enum;
using Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Role(Role.Admin)]
    public class TransactionController : BaseAPIController
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ResponseBase<Pagination<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            ResponseBase<Pagination<TransactionHistoryDTO>?> response = await _service.List(filter, WareHouseID, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{TransactionID}")]
        public async Task<ResponseBase<TransactionDetailDTO?>> Detail([Required] Guid TransactionID)
        {
            ResponseBase<TransactionDetailDTO?> response = await _service.Detail(TransactionID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
