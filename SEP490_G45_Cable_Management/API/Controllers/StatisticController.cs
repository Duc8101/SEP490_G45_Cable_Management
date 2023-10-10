using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.StatisticDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StatisticController : BaseAPIController
    {
        private readonly StatisticService service = new StatisticService();
        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<MaterialFluctuationPerYear?>> MaterialFluctuationPerYear(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.MaterialFluctuationPerYear(MaterialCategoryID, WarehouseID, year);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<CableFluctuationPerYear?>> CableFluctuationPerYear(int? CableCategoryID, int? WarehouseID, int? year)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.CableFluctuationPerYear(CableCategoryID, WarehouseID, year);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        [Authorize]
        public async Task<List<CableCategoryStatistic>> CableCategory(int? WarehouseID)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.CableCategory(WarehouseID);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        [Authorize]
        public async Task<List<OtherMaterialCateogoryStatistic>> MaterialCategory(int? WarehouseID)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.MaterialCategory(WarehouseID);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
