using DataAccess.DTO.TransactionDTO;
using DataAccess.DTO;
using DataAccess.Model.DAO;

namespace API.Services
{
    public class TransactionService
    {
        private readonly DAOTransaction daoTransaction = new DAOTransaction();
        public async Task<PagedResultDTO<TransactionListDTO>> List(string? filter, int? WareHouseID, int page)
        {

        }
    }
}
