using DataAccess.DTO.TransactionDTO;
using DataAccess.DTO;
using DataAccess.Model.DAO;
using DataAccess.Entity;
using Org.BouncyCastle.Utilities.Collections;
using DataAccess.Const;
using System.Net;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace API.Services
{
    public class TransactionService
    {
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAOWarehouse daoWare = new DAOWarehouse();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOCable daoCable = new DAOCable();
        private readonly DAOCableCategory daoCableCategory = new DAOCableCategory();
        private readonly DAOOtherMaterial daoMaterial = new DAOOtherMaterial();
        private readonly DAOOtherMaterialsCategory daoMaterialCategory = new DAOOtherMaterialsCategory();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();
        private async Task<string?> getIssueCode(Guid? IssueID)
        {
            // if transaction not exist issue
            if(IssueID == null)
            {
                return null;
            }
            Issue? issue = await daoIssue.getIssue(IssueID.Value);
            // if issue not exist
            if(issue == null)
            {
                return string.Empty;
            }
            return issue.IssueCode;
        }
        private async Task<string?> getWarehouseName(int? WarehouseID)
        {
            // if transaction not exist
            if (WarehouseID == null)
            {
                return null;
            }
            Warehouse? ware = await daoWare.getWarehouse(WarehouseID.Value);
            // if not exist ware house
            if(ware == null)
            {
                return string.Empty;
            }
            return ware.WarehouseName;
        }
        private async Task<List<TransactionHistoryDTO>> getListTransactionHistory(string? filter, int? WareHouseID, int page)
        {
            List<TransactionHistory> list = await daoHistory.getList(filter, WareHouseID, page);
            List<TransactionHistoryDTO> result = new List<TransactionHistoryDTO>();
            foreach (TransactionHistory history in list)
            {
                string? IssueCode = await getIssueCode(history.IssueId);
                string? FromWarehouseName = await getWarehouseName(history.FromWarehouseId);
                string? ToWarehouseName = await getWarehouseName(history.ToWarehouseId);
                // if get data successful
                if((IssueCode == null || IssueCode.Length != 0) && (FromWarehouseName == null || FromWarehouseName.Length != 0) && (ToWarehouseName == null || ToWarehouseName.Length != 0))
                {
                    TransactionHistoryDTO DTO = new TransactionHistoryDTO()
                    {
                        TransactionId = history.TransactionId,
                        TransactionCategoryName = history.TransactionCategoryName,
                        Description = history.Description,
                        CreatedAt = history.CreatedAt,
                        WarehouseId = history.WarehouseId,
                        IssueCode = IssueCode,
                        FromWarehouseName = FromWarehouseName,
                        ToWarehouseName = ToWarehouseName,
                        CreatedDate = history.CreatedDate,
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }
        private async Task<bool> isSuccessfulGetListHistory(string? filter, int? WareHouseID, int page)
        {
            List<TransactionHistory> list = await daoHistory.getList(filter, WareHouseID, page);
            List<TransactionHistoryDTO> result = await getListTransactionHistory(filter, WareHouseID, page);
            return list.Count == result.Count;
        }
        public async Task<ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>> List(string? filter, int? WareHouseID, int page)
        {
            List<TransactionHistoryDTO> list = await getListTransactionHistory(filter, WareHouseID, page);
            // if get data successful
            if(await isSuccessfulGetListHistory(filter, WareHouseID, page))
            {
                int RowCount = await daoHistory.getRowCount(filter, WareHouseID);
                PagedResultDTO<TransactionHistoryDTO> result = new PagedResultDTO<TransactionHistoryDTO>(page, RowCount, PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(result, string.Empty);
            }
            return new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(null, "Có lỗi xảy ra khi lấy dữ liệu", (int) HttpStatusCode.NotAcceptable);
        }

        private async Task<string?> getCableCategoryName(Guid CableID)
        {
            Cable? cable = await daoCable.getCableIncludeDeleted(CableID);
            if (cable == null)
            {
                return null;
            }
            int CategoryID = cable.CableCategoryId;
            CableCategory? category = await daoCableCategory.getCableCategory(CategoryID);
            return category == null ? string.Empty : category.CableCategoryName;
        }
        private async Task<List<TransactionCableDTO>> getListTransactionCable(Guid TransactionID)
        {
            List<TransactionCable> list = await daoTransactionCable.getList(TransactionID);
            List<TransactionCableDTO> result = new List<TransactionCableDTO>();
            foreach(TransactionCable transaction in list)
            {
                string? name = await getCableCategoryName(transaction.CableId);
                // if get name successful
                if(name != null && name.Length != 0)
                {
                    TransactionCableDTO DTO = new TransactionCableDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        CableId = transaction.CableId,
                        CableCategoryName = name,
                        StartPoint = transaction.StartPoint,
                        EndPoint = transaction.EndPoint,
                        Length = transaction.Length,
                        Note = transaction.Note,
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }

        private async Task<string?> getMaterialCategoryName(int OtherMaterialsID)
        {
            OtherMaterial? material = await daoMaterial.getOtherMaterialIncludeDeleted(OtherMaterialsID);
            if (material == null)
            {
                return null;
            }
            OtherMaterialsCategory? category = await daoMaterialCategory.getOtherMaterialsCategory(material.OtherMaterialsCategoryId);
            return category == null ? string.Empty : category.OtherMaterialsCategoryName;
        }

        private async Task<string?> getMaterialCode(int OtherMaterialsID)
        {
            OtherMaterial? material = await daoMaterial.getOtherMaterial(OtherMaterialsID);
            return material == null ? null : material.Code;
        }
        private async Task<List<TransactionMaterialDTO>> getListTransactionMaterial(Guid TransactionID)
        {
            List<TransactionOtherMaterial> list = await daoTransactionMaterial.getList(TransactionID);
            List<TransactionMaterialDTO> result = new List<TransactionMaterialDTO>();
            foreach (TransactionOtherMaterial transaction in list)
            {
                string? name = await getMaterialCategoryName(transaction.OtherMaterialsId);
                string? code = await getMaterialCode(transaction.OtherMaterialsId);
                // if get name successful
                if(name != null && name.Length != 0 && code != null)
                {
                    TransactionMaterialDTO DTO = new TransactionMaterialDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        OtherMaterialsId = transaction.OtherMaterialsId,
                        OtherMaterialsCategoryName = name,
                        Quantity = transaction.Quantity,
                        Code = code
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }

        private async Task<bool> isSuccessfulGetListCable(Guid TransactionID)
        {
            List<TransactionCable> list = await daoTransactionCable.getList(TransactionID);
            List<TransactionCableDTO> result = await getListTransactionCable(TransactionID);
            return list.Count == result.Count;
        }

        private async Task<bool> isSuccessfulGetListMaterial(Guid TransactionID)
        {
            List<TransactionOtherMaterial> list = await daoTransactionMaterial.getList(TransactionID);
            List<TransactionMaterialDTO> result = await getListTransactionMaterial(TransactionID);
            return list.Count == result.Count;
        }

        public async Task<ResponseDTO<TransactionDetailDTO?>> Detail(Guid TransactionID)
        {
            TransactionHistory? history = await daoHistory.getTransactionHistory(TransactionID);
            if(history == null)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, "Không tìm thấy giao dịch", (int) HttpStatusCode.NotFound);
            }
            // if get data trans cable fail
            if(await isSuccessfulGetListCable(TransactionID) == false)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, "Có lỗi lấy dữ liệu cáp giao dịch", (int) HttpStatusCode.NotAcceptable);
            }
            // if get data trans material fail
            if (await isSuccessfulGetListMaterial(TransactionID) == false)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, "Có lỗi lấy dữ liệu vật liệu giao dịch", (int)HttpStatusCode.NotAcceptable);
            }
            List<TransactionCableDTO> listCable = await getListTransactionCable(TransactionID);
            List<TransactionMaterialDTO> listMaterial = await getListTransactionMaterial(TransactionID);
            List<TransactionDetailDTO> listDetail = new List<TransactionDetailDTO>();
            string? IssueCode = await getIssueCode(history.IssueId);
            string? FromWarehouseName = await getWarehouseName(history.FromWarehouseId);
            string? ToWarehouseName = await getWarehouseName(history.ToWarehouseId);
            // if get data successful
            if ((IssueCode == null || IssueCode.Length != 0) && (FromWarehouseName == null || FromWarehouseName.Length != 0) && (ToWarehouseName == null || ToWarehouseName.Length != 0))
            {
                TransactionDetailDTO DTO = new TransactionDetailDTO()
                {
                    TransactionId = history.TransactionId,
                    TransactionCategoryName = history.TransactionCategoryName,
                    Description = history.Description,
                    CreatedAt = history.CreatedAt,
                    WarehouseId = history.WarehouseId,
                    IssueCode = IssueCode,
                    FromWarehouseName = FromWarehouseName,
                    ToWarehouseName = ToWarehouseName,
                    CreatedDate = history.CreatedDate,
                    CableTransactions = listCable,
                    MaterialsTransaction = listMaterial,
                };
                listDetail.Add(DTO);
            }
            // if get data failed
            if(listDetail.Count == 0)
            {
                return new ResponseDTO<TransactionDetailDTO?>(null, "Có lỗi lấy dữ liệu chi tiết giao dịch", (int)HttpStatusCode.NotAcceptable);
            }
            return new ResponseDTO<TransactionDetailDTO?>(listDetail[0], "");
        }
    }
}
