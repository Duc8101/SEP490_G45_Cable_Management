using Common.Base;
using Common.DTO.TransactionDTO;
using Common.Pagination;

namespace API.Services.Transaction
{
    public interface ITransactionService
    {
        Task<ResponseBase<Pagination<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page);
        Task<ResponseBase<TransactionDetailDTO?>> Detail(Guid TransactionID);
    }
}
