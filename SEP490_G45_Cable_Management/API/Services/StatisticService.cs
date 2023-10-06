using DataAccess.DTO;
using DataAccess.DTO.StatisticDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using System.Net;

namespace API.Services
{
    public class StatisticService
    {
        private readonly DAOTransactionOtherMaterial daoTransactionOtherMaterial = new DAOTransactionOtherMaterial();
        private readonly DAOOtherMaterial daoOtherMaterial = new DAOOtherMaterial();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOCable daoCable = new DAOCable();
        public async Task<ResponseDTO<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            MaterialFluctuationPerYear data = new MaterialFluctuationPerYear();
            // if choose category
            if (MaterialCategoryID != null)
            {
                List<OtherMaterial> list = await daoOtherMaterial.getList(MaterialCategoryID.Value);
                // if not found
                if (list.Count == 0)
                {
                    return new ResponseDTO<MaterialFluctuationPerYear?>(null, "Không tìm thấy vật liệu", (int) HttpStatusCode.NotFound);
                }
                data.MaterialName = list[0].OtherMaterialsCategory.OtherMaterialsCategoryName;
            }
            data.WarehouseId = WarehouseID;
            data.QuantityInJanuary = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 1);
            data.QuantityInFebruary = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 2);
            data.QuantityInMarch = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 3);
            data.QuantityInApril = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 4);
            data.QuantityInMay = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 5);
            data.QuantityInJune = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 6);
            data.QuantityInJuly = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 7);
            data.QuantityInAugust = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 8);
            data.QuantityInSeptember = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 9);
            data.QuantityInOctober = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 10);
            data.QuantityInNovember = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 11);
            data.QuantityInDecember = await daoTransactionOtherMaterial.getQuantityPerMonth(MaterialCategoryID, WarehouseID, year, 12);
            return new ResponseDTO<MaterialFluctuationPerYear?>(data, "");
        }
        public async Task<ResponseDTO<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            CableFluctuationPerYear data = new CableFluctuationPerYear();
            // if choose category
            if(CableCategoryID != null)
            {
                List<Cable> list = await daoCable.getList(CableCategoryID.Value);
                // if not found
                if(list.Count == 0)
                {
                    return new ResponseDTO<CableFluctuationPerYear?>(null, "Không tìm thấy cáp", (int) HttpStatusCode.NotFound);
                }
                data.CableName = list[0].CableCategory.CableCategoryName;
            }
            data.WarehouseId = WarehouseID;
            data.LengthInJanuary = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 1);
            data.LengthInFebruary = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 2);
            data.LengthInMarch = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 3);
            data.LengthInApril = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 4);
            data.LengthInMay = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 5);
            data.LengthInJune = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 6);
            data.LengthInJuly = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 7);
            data.LengthInAugust = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 8);
            data.LengthInSeptember = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 9);
            data.LengthInOctober = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 10);
            data.LengthInNovember = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 11);
            data.LengthInDecember = await daoTransactionCable.getLengthPerMonth(CableCategoryID, WarehouseID, year, 12);
            return new ResponseDTO<CableFluctuationPerYear?>(data, "");
        }
    }
}
