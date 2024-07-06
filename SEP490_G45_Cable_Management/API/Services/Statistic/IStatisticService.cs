using Common.Base;

namespace API.Services.Statistic
{
    public interface IStatisticService
    {
        ResponseBase MaterialFluctuationPerYear(int? otherMaterialCategoryId, int? warehouseId, int? year);
        ResponseBase CableFluctuationPerYear(int? cableCategoryId, int? warehouseId, int? year);
        ResponseBase CableCategory(int? warehouseId);
        ResponseBase MaterialCategory(int? warehouseId);
        ResponseBase Route(Guid routeId);
    }
}
