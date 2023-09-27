﻿using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SupplierController : BaseAPIController
    {
        private readonly SupplierService service = new SupplierService();
        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO)
        { 
            // if admin
            if (isAdmin())
            {
                // get user id
                string? CreatorID = getUserID();
                // if not found user id
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn", (int) HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, CreatorID);             
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        [Authorize]
        public async Task<PagedResultDTO<SupplierListDTO>> List(string? filter /* list supplier based on supplier name */, int page /* current page */)
        {
            // if admin or warehouse
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.List(filter, page);
            }
            throw new UnauthorizedAccessException();
        }

        [HttpPut("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(SupplierID, DTO);  
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Delete(SupplierID);
            }
             throw new UnauthorizedAccessException();  
        }
    }
}
