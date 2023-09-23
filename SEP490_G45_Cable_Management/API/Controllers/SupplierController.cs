using API.Services;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
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
            // if admin or warehouse
            if (isAdmin() || isWarehouseKeeper())
            {
                // get user id
                string? CreatorID = getUserID();
                // if not found user id
                if (CreatorID == null)
                {
                    return new ResponseDTO<bool>("Không tìm thấy ID của bạn", (int) HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, CreatorID);             
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        public async Task<PagedResultDTO<SupplierListDTO>> List(string? filter /* list supplier based on supplier name */, int pageSize = 12 /* number of row in a page */, int currentPage = 1 )
        {
            return await service.List(filter, pageSize, currentPage);
        }

        [HttpPut("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            // if admin or warehouse
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Update(SupplierID, DTO);  
            }
            throw new UnauthorizedAccessException();
        }

        [HttpDelete("{SupplierID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            // if admin or warehousekeeper
            if (isAdmin() || isWarehouseKeeper())
            {
                return await service.Delete(SupplierID);
            }
             throw new UnauthorizedAccessException();  
        }
    }
}
