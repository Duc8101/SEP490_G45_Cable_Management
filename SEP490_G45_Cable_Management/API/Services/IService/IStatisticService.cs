using DataAccess.DTO.StatisticDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IStatisticService
    {
        Task<ResponseDTO<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year);
        Task<ResponseDTO<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year);
        Task<ResponseDTO<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID);
        Task<ResponseDTO<List<OtherMaterialCateogoryStatistic>?>> MaterialCategory(int? WarehouseID);
    }
}
