using API.Attributes;
using API.Services.IService;
using Common.Base;
using Common.DTO.StatisticDTO;
using Common.Enum;
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
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            ResponseBase<MaterialFluctuationPerYear?> response = await _service.MaterialFluctuationPerYear(MaterialCategoryID, WarehouseID, year);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            ResponseBase<CableFluctuationPerYear?> response = await _service.CableFluctuationPerYear(CableCategoryID, WarehouseID, year); ;
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID)
        {
            ResponseBase<List<CableCategoryStatistic>?> response = await _service.CableCategory(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Role.Admin, Role.Leader, Role.Warehouse_Keeper)]
        public async Task<ResponseBase<List<OtherMaterialCategoryStatistic>?>> MaterialCategory(int? WarehouseID)
        {
            ResponseBase<List<OtherMaterialCategoryStatistic>?> response = await _service.MaterialCategory(WarehouseID);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        public async Task<ResponseBase<List<RouteStatistic>?>> Route([Required] Guid RouteID)
        {
            ResponseBase<List<RouteStatistic>?> response = await _service.Route(RouteID);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
