using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.StatisticDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StatisticController : BaseAPIController
    {
        private readonly IStatisticService service;

        public StatisticController(IStatisticService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            // if admin or leader , ware keeper
            if (isAdmin() || isLeader() || isWarehouseKeeper())
            {
                return await service.MaterialFluctuationPerYear(MaterialCategoryID, WarehouseID, year);
            }
            return new ResponseDTO<MaterialFluctuationPerYear?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            // if admin or leader , ware keeper
            if (isAdmin() || isLeader() || isWarehouseKeeper())
            {
                return await service.CableFluctuationPerYear(CableCategoryID, WarehouseID, year);
            }
            return new ResponseDTO<CableFluctuationPerYear?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID)
        {
            // if admin or leader , ware keeper
            if (isAdmin() || isLeader() || isWarehouseKeeper())
            {
                return await service.CableCategory(WarehouseID);
            }
            return new ResponseDTO<List<CableCategoryStatistic>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<OtherMaterialCategoryStatistic>?>> MaterialCategory(int? WarehouseID)
        {
            // if admin or leader , ware keeper
            if (isAdmin() || isLeader() || isWarehouseKeeper())
            {
                return await service.MaterialCategory(WarehouseID);
            }
            return new ResponseDTO<List<OtherMaterialCategoryStatistic>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<RouteStatistic>?>> Route([Required] Guid RouteID)
        {
            // if admin or leader , ware keeper
            if (isAdmin() || isLeader() || isWarehouseKeeper())
            {
                return await service.Route(RouteID);
            }
            return new ResponseDTO<List<RouteStatistic>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
