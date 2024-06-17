using Common.Base;
using Common.DTO.StatisticDTO;

namespace API.Services.IService
{
    public interface IStatisticService
    {
        Task<ResponseBase<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year);
        Task<ResponseBase<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year);
        Task<ResponseBase<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID);
        Task<ResponseBase<List<OtherMaterialCategoryStatistic>?>> MaterialCategory(int? WarehouseID);
        Task<ResponseBase<List<RouteStatistic>?>> Route(Guid RouteID);
    }
}
