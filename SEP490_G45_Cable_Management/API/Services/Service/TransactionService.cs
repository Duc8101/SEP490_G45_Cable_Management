using API.Model.DAO;
using API.Services.IService;
using AutoMapper;
using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.TransactionDTO;
using DataAccess.Entity;
using System.Net;

namespace API.Services.Service
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();

        public TransactionService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page)
        {
            try
            {
                List<TransactionHistory> list = await daoHistory.getList(filter, WareHouseID, page);
                List<TransactionHistoryDTO> DTOs = _mapper.Map<List<TransactionHistoryDTO>>(list);
                int RowCount = await daoHistory.getRowCount(filter, WareHouseID);
                PagedResultDTO<TransactionHistoryDTO> result = new PagedResultDTO<TransactionHistoryDTO>(page, RowCount, PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail(Guid TransactionID)
        {
            try
            {
                TransactionHistory? history = await daoHistory.getTransactionHistory(TransactionID);
                if (history == null)
                {
                    return new ResponseDTO<TransactionDetailDTO?>(null, "Không tìm thấy giao dịch", (int)HttpStatusCode.NotFound);
                }
                List<TransactionCable> listCable = await daoTransactionCable.getList(TransactionID);
                List<TransactionCableDTO> cableDTOs = _mapper.Map<List<TransactionCableDTO>>(listCable);
                List<TransactionOtherMaterial> listMaterial = await daoTransactionMaterial.getList(TransactionID);
                List<TransactionMaterialDTO> materialDTOs = _mapper.Map<List<TransactionMaterialDTO>>(listMaterial);
                TransactionDetailDTO data = _mapper.Map<TransactionDetailDTO>(history);
                data.CableTransactions = cableDTOs;
                data.MaterialsTransaction = materialDTOs;
                return new ResponseDTO<TransactionDetailDTO?>(data, "");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
