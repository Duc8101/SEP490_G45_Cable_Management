using API.Attributes;
using API.Services.Statistic;
using Common.Base;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
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
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase MaterialFluctuationPerYear(int? otherMaterialCategoryId, int? warehouseId, int? year)
        {
            ResponseBase response = _service.MaterialFluctuationPerYear(otherMaterialCategoryId, warehouseId, year);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase CableFluctuationPerYear(int? cableCategoryId, int? warehouseId, int? year)
        {
            ResponseBase response = _service.CableFluctuationPerYear(cableCategoryId, warehouseId, year);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase CableCategory(int? warehouseId)
        {
            ResponseBase response = _service.CableCategory(warehouseId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        [Role(Roles.Admin, Roles.Leader, Roles.Warehouse_Keeper)]
        public ResponseBase MaterialCategory(int? warehouseId)
        {
            ResponseBase response = _service.MaterialCategory(warehouseId);
            Response.StatusCode = response.Code;
            return response;
        }

        [HttpGet]
        public ResponseBase Route([Required] Guid routeId)
        {
            ResponseBase response = _service.Route(routeId);
            Response.StatusCode = response.Code;
            return response;
        }
    }
}
