using Common.Base;

namespace API.Services.Transaction
{
    public interface ITransactionService
    {
        ResponseBase List(string? filter, int? wareHouseId, int page);
        ResponseBase Detail(Guid transactionId);
    }
}
