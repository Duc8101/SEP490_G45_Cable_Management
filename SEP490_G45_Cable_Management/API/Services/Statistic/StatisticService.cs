using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.StatisticDTO;
using Common.Entity;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Statistic
{
    public class StatisticService : BaseService, IStatisticService
    {
        private readonly DAOTransactionOtherMaterial _daoTransactionOtherMaterial;
        private readonly DAOOtherMaterial _daoOtherMaterial;
        private readonly DAOTransactionCable _daoTransactionCable;
        private readonly DAOCable _daoCable;
        private readonly DAORoute _daoRoute;
        private readonly DAONodeMaterialCategory _daoNodeMaterialCategory;

        public StatisticService(IMapper mapper, DAOTransactionOtherMaterial daoTransactionOtherMaterial, DAOOtherMaterial daoOtherMaterial, DAOTransactionCable daoTransactionCable, DAOCable daoCable, DAORoute daoRoute, DAONodeMaterialCategory daoNodeMaterialCategory) : base(mapper)
        {
            _daoTransactionOtherMaterial = daoTransactionOtherMaterial;
            _daoOtherMaterial = daoOtherMaterial;
            _daoTransactionCable = daoTransactionCable;
            _daoCable = daoCable;
            _daoRoute = daoRoute;
            _daoNodeMaterialCategory = daoNodeMaterialCategory;
        }

        public ResponseBase CableCategory(int? warehouseId)
        {
            try
            {
                // get list cable category
                List<CableCategory> list = _daoCable.getListCableCategory(warehouseId);
                List<CableCategoryStatistic> data = new List<CableCategoryStatistic>();
                foreach (CableCategory category in list)
                {
                    // get sum of length
                    int sumLength = _daoCable.getSum(category.CableCategoryId, warehouseId);
                    CableCategoryStatistic statistic = new CableCategoryStatistic()
                    {
                        CableCategoryId = category.CableCategoryId,
                        CableCategoryName = category.CableCategoryName,
                        SumOfLength = sumLength,
                    };
                    data.Add(statistic);
                }
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase CableFluctuationPerYear(int? cableCategoryId, int? warehouseId, int? year)
        {
            CableFluctuationPerYear data = new CableFluctuationPerYear();
            try
            {
                // if choose category
                if (cableCategoryId.HasValue)
                {
                    Cable? cable = _daoCable.getCable(cableCategoryId.Value);
                    // if not found
                    if (cable == null)
                    {
                        return new ResponseBase("Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                    }
                    data.CableName = cable.CableCategory.CableCategoryName;
                }
                data.WarehouseId = warehouseId;
                data.LengthInJanuary = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 1);
                data.LengthInFebruary = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 2);
                data.LengthInMarch = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 3);
                data.LengthInApril = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 4);
                data.LengthInMay = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 5);
                data.LengthInJune = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 6);
                data.LengthInJuly = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 7);
                data.LengthInAugust = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 8);
                data.LengthInSeptember = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 9);
                data.LengthInOctober = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 10);
                data.LengthInNovember = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 11);
                data.LengthInDecember = _daoTransactionCable.getLengthPerMonth(cableCategoryId, warehouseId, year, 12);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase MaterialCategory(int? warehouseId)
        {
            try
            {
                List<OtherMaterialsCategory> list = _daoOtherMaterial.getListOtherMaterialCategory(warehouseId);
                List<OtherMaterialCategoryStatistic> data = new List<OtherMaterialCategoryStatistic>();
                foreach (OtherMaterialsCategory material in list)
                {
                    int sum = _daoOtherMaterial.getSum(material.OtherMaterialsCategoryId, warehouseId);
                    OtherMaterialCategoryStatistic statistic = new OtherMaterialCategoryStatistic()
                    {
                        CategoryId = material.OtherMaterialsCategoryId,
                        CategoryName = material.OtherMaterialsCategoryName,
                        SumOfQuantity = sum
                    };
                    data.Add(statistic);
                }
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase MaterialFluctuationPerYear(int? otherMaterialCategoryId, int? warehouseId, int? year)
        {
            MaterialFluctuationPerYear data = new MaterialFluctuationPerYear();
            try
            {
                // if choose category
                if (otherMaterialCategoryId.HasValue)
                {
                    List<OtherMaterial> list = _daoOtherMaterial.getListOtherMaterial(otherMaterialCategoryId.Value);
                    // if not found
                    if (list.Count == 0)
                    {
                        return new ResponseBase("Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                    }
                    data.MaterialName = list[0].OtherMaterialsCategory.OtherMaterialsCategoryName;
                }
                data.WarehouseId = warehouseId;
                data.QuantityInJanuary = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 1);
                data.QuantityInFebruary = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 2);
                data.QuantityInMarch = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 3);
                data.QuantityInApril = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 4);
                data.QuantityInMay = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 5);
                data.QuantityInJune = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 6);
                data.QuantityInJuly = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 7);
                data.QuantityInAugust = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 8);
                data.QuantityInSeptember = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 9);
                data.QuantityInOctober = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 10);
                data.QuantityInNovember = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 11);
                data.QuantityInDecember = _daoTransactionOtherMaterial.getQuantityPerMonth(otherMaterialCategoryId, warehouseId, year, 12);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Route(Guid routeId)
        {
            try
            {
                Common.Entity.Route? route = _daoRoute.getRoute(routeId);
                if (route == null)
                {
                    return new ResponseBase("Không tìm thấy tuyến", (int)HttpStatusCode.NotFound);
                }
                List<RouteStatistic> data = new List<RouteStatistic>();
                List<OtherMaterialsCategory> listCategory = _daoNodeMaterialCategory.getListOtherMaterialCategory(routeId);
                foreach (OtherMaterialsCategory item in listCategory)
                {
                    int sum = _daoNodeMaterialCategory.getSumQuantity(routeId, item.OtherMaterialsCategoryId);
                    RouteStatistic statistic = new RouteStatistic()
                    {
                        OtherMaterialsCategoryId = item.OtherMaterialsCategoryId,
                        OtherMaterialsCategoryName = item.OtherMaterialsCategoryName,
                        Quantity = sum
                    };
                    data.Add(statistic);
                }
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
