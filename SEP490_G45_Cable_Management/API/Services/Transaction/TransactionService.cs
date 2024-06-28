using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.TransactionDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Transaction
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();

        public TransactionService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page)
        {
            try
            {
                List<TransactionHistory> list = await daoHistory.getList(filter, WareHouseID, page);
                List<TransactionHistoryDTO> DTOs = _mapper.Map<List<TransactionHistoryDTO>>(list);
                int RowCount = await daoHistory.getRowCount(filter, WareHouseID);
                Pagination<TransactionHistoryDTO> result = new Pagination<TransactionHistoryDTO>(page, RowCount, PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<TransactionHistoryDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<TransactionHistoryDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<TransactionDetailDTO?>> Detail(Guid TransactionID)
        {
            try
            {
                TransactionHistory? history = await daoHistory.getTransactionHistory(TransactionID);
                if (history == null)
                {
                    return new ResponseBase<TransactionDetailDTO?>(null, "Không tìm thấy giao dịch", (int)HttpStatusCode.NotFound);
                }
                List<TransactionCable> listCable = await daoTransactionCable.getList(TransactionID);
                List<TransactionCableDTO> cableDTOs = _mapper.Map<List<TransactionCableDTO>>(listCable);
                List<TransactionOtherMaterial> listMaterial = await daoTransactionMaterial.getList(TransactionID);
                List<TransactionMaterialDTO> materialDTOs = _mapper.Map<List<TransactionMaterialDTO>>(listMaterial);
                TransactionDetailDTO data = _mapper.Map<TransactionDetailDTO>(history);
                data.CableTransactions = cableDTOs;
                data.MaterialsTransaction = materialDTOs;
                return new ResponseBase<TransactionDetailDTO?>(data, "");
            }
            catch (Exception ex)
            {
                return new ResponseBase<TransactionDetailDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
