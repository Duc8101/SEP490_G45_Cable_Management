using API.Attributes;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.StatisticDTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class StatisticController : BaseAPIController
    {
        private readonly IStatisticService _service;

        public StatisticController(IStatisticService service)
        {
            _service = service;
        }

        [HttpGet]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            ResponseDTO<MaterialFluctuationPerYear?> response = await _service.MaterialFluctuationPerYear(MaterialCategoryID, WarehouseID, year);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            ResponseDTO<CableFluctuationPerYear?> response = await _service.CableFluctuationPerYear(CableCategoryID, WarehouseID, year); ;
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID)
        {
            ResponseDTO<List<CableCategoryStatistic>?> response = await _service.CableCategory(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(DataAccess.Enum.Role.Admin, DataAccess.Enum.Role.Leader, DataAccess.Enum.Role.Warehouse_Keeper)]
        public async Task<ResponseDTO<List<OtherMaterialCategoryStatistic>?>> MaterialCategory(int? WarehouseID)
        {
            ResponseDTO<List<OtherMaterialCategoryStatistic>?> response = await _service.MaterialCategory(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        public async Task<ResponseDTO<List<RouteStatistic>?>> Route([Required] Guid RouteID)
        {
            ResponseDTO<List<RouteStatistic>?> response = await _service.Route(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
