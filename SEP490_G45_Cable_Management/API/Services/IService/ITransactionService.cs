using DataAccess.DTO.TransactionDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface ITransactionService
    {
        Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page);
        Task<ResponseDTO<TransactionDetailDTO?>> Detail(Guid TransactionID);
    }
}
