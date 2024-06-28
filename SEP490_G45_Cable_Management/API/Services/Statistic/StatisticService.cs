using System.Net;
using Common.Base;
using Common.DTO.StatisticDTO;
using Common.Entity;
using DataAccess.DAO;

namespace API.Services.Statistic
{
    public class StatisticService : IStatisticService
    {
        private readonly DAOTransactionOtherMaterial daoTransactionOtherMaterial = new DAOTransactionOtherMaterial();
        private readonly DAOOtherMaterial daoOtherMaterial = new DAOOtherMaterial();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOCable daoCable = new DAOCable();
        private readonly DAORoute daoRoute = new DAORoute();
        private readonly DAONodeMaterialCategory daoNodeCategory = new DAONodeMaterialCategory();
        public async Task<ResponseBase<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            MaterialFluctuationPerYear data = new MaterialFluctuationPerYear();
            try
            {
                // if choose category
                if (MaterialCategoryID.HasValue)
                {
                    List<OtherMaterial> list = await daoOtherMaterial.getListAll(MaterialCategoryID.Value);
                    // if not found
                    if (list.Count == 0)
                    {
                        return new ResponseBase<MaterialFluctuationPerYear?>(null, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
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
                return new ResponseBase<MaterialFluctuationPerYear?>(data, "");
            }
            catch (Exception ex)
            {
                return new ResponseBase<MaterialFluctuationPerYear?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }
        public async Task<ResponseBase<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            CableFluctuationPerYear data = new CableFluctuationPerYear();
            try
            {
                // if choose category
                if (CableCategoryID.HasValue)
                {
                    Cable? cable = await daoCable.getCable(CableCategoryID.Value);
                    // if not found
                    if (cable == null)
                    {
                        return new ResponseBase<CableFluctuationPerYear?>(null, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                    }
                    data.CableName = cable.CableCategory.CableCategoryName;
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
                return new ResponseBase<CableFluctuationPerYear?>(data, "");
            }
            catch (Exception ex)
            {
                return new ResponseBase<CableFluctuationPerYear?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }
        public async Task<ResponseBase<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID)
        {
            try
            {
                // get list cable category
                List<CableCategory> list = await daoCable.getListCategory(WarehouseID);
                List<CableCategoryStatistic> result = new List<CableCategoryStatistic>();
                foreach (CableCategory category in list)
                {
                    // get sum of length
                    int sumLength = await daoCable.getSum(category.CableCategoryId, WarehouseID);
                    CableCategoryStatistic statistic = new CableCategoryStatistic()
                    {
                        CableCategoryId = category.CableCategoryId,
                        CableCategoryName = category.CableCategoryName,
                        SumOfLength = sumLength,
                    };
                    result.Add(statistic);
                }
                return new ResponseBase<List<CableCategoryStatistic>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<CableCategoryStatistic>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<OtherMaterialCategoryStatistic>?>> MaterialCategory(int? WarehouseID)
        {
            try
            {
                List<OtherMaterialsCategory> list = await daoOtherMaterial.getListCategory(WarehouseID);
                List<OtherMaterialCategoryStatistic> result = new List<OtherMaterialCategoryStatistic>();
                foreach (OtherMaterialsCategory material in list)
                {
                    int sum = await daoOtherMaterial.getSum(material.OtherMaterialsCategoryId, WarehouseID);
                    OtherMaterialCategoryStatistic statistic = new OtherMaterialCategoryStatistic()
                    {
                        CategoryId = material.OtherMaterialsCategoryId,
                        CategoryName = material.OtherMaterialsCategoryName,
                        SumOfQuantity = sum
                    };
                    result.Add(statistic);
                }
                return new ResponseBase<List<OtherMaterialCategoryStatistic>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<OtherMaterialCategoryStatistic>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<RouteStatistic>?>> Route(Guid RouteID)
        {
            try
            {
                Common.Entity.Route? route = await daoRoute.getRoute(RouteID);
                if (route == null)
                {
                    return new ResponseBase<List<RouteStatistic>?>(null, "Không tìm thấy tuyến", (int)HttpStatusCode.NotFound);
                }
                List<RouteStatistic> result = new List<RouteStatistic>();
                List<OtherMaterialsCategory> listCategory = await daoNodeCategory.getListOtherMaterialCategory(RouteID);
                foreach (OtherMaterialsCategory item in listCategory)
                {
                    int sum = await daoNodeCategory.getSumQuantity(RouteID, item.OtherMaterialsCategoryId);
                    RouteStatistic statistic = new RouteStatistic()
                    {
                        OtherMaterialsCategoryId = item.OtherMaterialsCategoryId,
                        OtherMaterialsCategoryName = item.OtherMaterialsCategoryName,
                        Quantity = sum
                    };
                    result.Add(statistic);
                }
                return new ResponseBase<List<RouteStatistic>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<RouteStatistic>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
