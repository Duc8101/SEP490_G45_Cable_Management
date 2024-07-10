using API.Attributes;
using API.Services.Transaction;
using Common.Base;
using Common.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Role(RoleConst.Admin)]
    public class TransactionController : BaseAPIController
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public ResponseBase List(string? filter, int? wareHouseId, [Required] int page = 1)
        {
            ResponseBase response = _service.List(filter, wareHouseId, page);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet("{transactionId}")]
        public ResponseBase Detail([Required] Guid transactionId)
        {
            ResponseBase response = _service.Detail(transactionId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
