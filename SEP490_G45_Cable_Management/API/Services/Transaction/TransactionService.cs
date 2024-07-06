using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.TransactionDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Transaction
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly DAOTransactionHistory _daoTransactionHistory;
        private readonly DAOTransactionCable _daoTransactionCable;
        private readonly DAOTransactionOtherMaterial _daoTransactionMaterial;

        public TransactionService(IMapper mapper, DAOTransactionHistory daoTransactionHistory, DAOTransactionCable daoTransactionCable
            , DAOTransactionOtherMaterial daoTransactionMaterial) : base(mapper)
        {
            _daoTransactionHistory = daoTransactionHistory;
            _daoTransactionCable = daoTransactionCable;
            _daoTransactionMaterial = daoTransactionMaterial;
        }

        public ResponseBase Detail(Guid transactionId)
        {
            try
            {
                TransactionHistory? history = _daoTransactionHistory.getTransactionHistory(transactionId);
                if (history == null)
                {
                    return new ResponseBase("Không tìm thấy giao dịch", (int)HttpStatusCode.NotFound);
                }
                List<TransactionCable> listCable = _daoTransactionCable.getListTransactionCable(transactionId);
                List<TransactionCableDTO> cableDTOs = _mapper.Map<List<TransactionCableDTO>>(listCable);
                List<TransactionOtherMaterial> listMaterial = _daoTransactionMaterial.getListTransactionOtherMaterial(transactionId);
                List<TransactionMaterialDTO> materialDTOs = _mapper.Map<List<TransactionMaterialDTO>>(listMaterial);
                TransactionDetailDTO data = _mapper.Map<TransactionDetailDTO>(history);
                data.CableTransactions = cableDTOs;
                data.MaterialsTransaction = materialDTOs;
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase List(string? filter, int? wareHouseId, int page)
        {
            try
            {
                List<TransactionHistory> list = _daoTransactionHistory.getListTransactionHistory(filter, wareHouseId, page);
                List<TransactionHistoryDTO> DTOs = _mapper.Map<List<TransactionHistoryDTO>>(list);
                int rowCount = _daoTransactionHistory.getRowCount(filter, wareHouseId);
                Pagination<TransactionHistoryDTO> data = new Pagination<TransactionHistoryDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    List = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
