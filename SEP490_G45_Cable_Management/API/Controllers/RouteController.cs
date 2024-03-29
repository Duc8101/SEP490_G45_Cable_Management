﻿using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class RouteController : BaseAPIController
    {
        private readonly IRouteService service;

        public RouteController(IRouteService service)
        {
            this.service = service;
        }


        [HttpGet("All")]
        [Authorize]
        public async Task<ResponseDTO<List<RouteListDTO>?>> List(string? name)
        {
            return await service.ListAll(name);
        }

        [HttpGet("Paged")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<RouteListDTO>?>> List([Required] int page = 1)
        {
            // if admin
            if (isAdmin())
            {
                return await service.ListPaged(page);
            }
            return new ResponseDTO<PagedResultDTO<RouteListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] RouteCreateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{RouteID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid RouteID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(RouteID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
