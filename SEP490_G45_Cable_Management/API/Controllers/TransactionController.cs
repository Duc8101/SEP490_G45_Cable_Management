using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.TransactionDTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Role(DataAccess.Enum.Role.Admin)]
    public class TransactionController : BaseAPIController
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, [Required] int page = 1)
        {
            ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?> response = await _service.List(filter, WareHouseID, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{TransactionID}")]
        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail([Required] Guid TransactionID)
        {
            ResponseDTO<TransactionDetailDTO?> response = await _service.Detail(TransactionID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
