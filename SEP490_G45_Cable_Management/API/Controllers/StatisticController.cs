using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.StatisticDTO;
using DataAccess.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            return new ResponseDTO<MaterialFluctuationPerYear?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
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
            return new ResponseDTO<CableFluctuationPerYear?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<CableCategoryStatistic>?>> CableCategory(int? WarehouseID)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.CableCategory(WarehouseID);
            }
            return new ResponseDTO<List<CableCategoryStatistic>?>(null, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseDTO<List<OtherMaterialCateogoryStatistic>?>> MaterialCategory(int? WarehouseID)
        {
            // if admin or leader
            if(isAdmin() || isLeader())
            {
                return await service.MaterialCategory(WarehouseID);
            }
            return new ResponseDTO<List<OtherMaterialCateogoryStatistic>?>(null, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
