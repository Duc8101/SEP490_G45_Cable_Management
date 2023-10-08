using DataAccess.DTO.TransactionDTO;
using DataAccess.DTO;
using API.Model.DAO;
using DataAccess.Entity;
using Org.BouncyCastle.Utilities.Collections;
using DataAccess.Const;
using System.Net;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.Services
{
    public class TransactionService
    {
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();
        private async Task<List<TransactionHistoryDTO>> getListTransactionHistory(string? filter, int? WareHouseID, int page)
        {
            List<TransactionHistory> list = await daoHistory.getList(filter, WareHouseID, page);
            List<TransactionHistoryDTO> result = new List<TransactionHistoryDTO>();
            foreach (TransactionHistory history in list)
            {
                TransactionHistoryDTO DTO = new TransactionHistoryDTO()
                {
                    TransactionId = history.TransactionId,
                    TransactionCategoryName = history.TransactionCategoryName,
                    Description = history.Description,
                    CreatedAt = history.CreatedAt,
                    WarehouseId = history.WarehouseId,
                    IssueCode = history.Issue == null ? null : history.Issue.IssueCode,
                    FromWarehouseName = history.FromWarehouse == null ? null : history.FromWarehouse.WarehouseName,
                    ToWarehouseName = history.ToWarehouse == null ? null : history.ToWarehouse.WarehouseName,
                    CreatedDate = history.CreatedDate,
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<TransactionHistoryDTO>> List(string? filter, int? WareHouseID, int page)
        {
            List<TransactionHistoryDTO> list = await getListTransactionHistory(filter, WareHouseID, page);
            int RowCount = await daoHistory.getRowCount(filter, WareHouseID);
            return new PagedResultDTO<TransactionHistoryDTO>(page, RowCount, PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE, list);
        }
        private async Task<List<TransactionCableDTO>> getListTransactionCable(Guid TransactionID)
        {
            List<TransactionCable> list = await daoTransactionCable.getList(TransactionID);
            List<TransactionCableDTO> result = new List<TransactionCableDTO>();
            foreach(TransactionCable transaction in list)
            {
                TransactionCableDTO DTO = new TransactionCableDTO()
                {
                    TransactionId = transaction.TransactionId,
                    CableId = transaction.CableId,
                    CableCategoryName = transaction.Cable.CableCategory.CableCategoryName,
                    StartPoint = transaction.StartPoint,
                    EndPoint = transaction.EndPoint,
                    Length = transaction.Length,
                    Note = transaction.Note,
                };
                result.Add(DTO);
            }
            return result;
        }
        private async Task<List<TransactionMaterialDTO>> getListTransactionMaterial(Guid TransactionID)
        {
            List<TransactionOtherMaterial> list = await daoTransactionMaterial.getList(TransactionID);
            List<TransactionMaterialDTO> result = new List<TransactionMaterialDTO>();
            foreach (TransactionOtherMaterial transaction in list)
            {
                TransactionMaterialDTO DTO = new TransactionMaterialDTO()
                {
                    TransactionId = transaction.TransactionId,
                    OtherMaterialsId = transaction.OtherMaterialsId,
                    OtherMaterialsCategoryName = transaction.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName,
                    Quantity = transaction.Quantity,
                    Code = transaction.OtherMaterials.Code
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail(Guid TransactionID)
        {
            TransactionHistory? history = await daoHistory.getTransactionHistory(TransactionID);
            if(history == null)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, "Không tìm thấy giao dịch", (int) HttpStatusCode.NotFound);
            }
            List<TransactionCableDTO> listCable = await getListTransactionCable(TransactionID);
            List<TransactionMaterialDTO> listMaterial = await getListTransactionMaterial(TransactionID);
            TransactionDetailDTO DTO = new TransactionDetailDTO()
            {
                TransactionId = history.TransactionId,
                TransactionCategoryName = history.TransactionCategoryName,
                Description = history.Description,
                CreatedAt = history.CreatedAt,
                WarehouseId = history.WarehouseId,
                IssueCode = history.Issue == null ? null : history.Issue.IssueCode,
                FromWarehouseName = history.FromWarehouse == null ? null : history.FromWarehouse.WarehouseName,
                ToWarehouseName = history.ToWarehouse == null ? null : history.ToWarehouse.WarehouseName,
                CreatedDate = history.CreatedDate,
                CableTransactions = listCable,
                MaterialsTransaction = listMaterial,
            };
            return new ResponseDTO<TransactionDetailDTO?>(DTO, "");
        }
    }
}
