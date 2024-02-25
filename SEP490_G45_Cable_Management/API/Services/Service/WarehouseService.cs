using DataAccess.DTO.WarehouseDTO;
using DataAccess.DTO;
using System.ComponentModel;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using API.Model.DAO;
using API.Services.IService;
using AutoMapper;

namespace API.Services.Service
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();

        public WarehouseService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseDTO<PagedResultDTO<WarehouseListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<Warehouse> list = await daoWarehouse.getListPaged(name, page);
                List<WarehouseListDTO> DTOs = mapper.Map<List<WarehouseListDTO>>(list);
                int RowCount = await daoWarehouse.getRowCount(name);
                PagedResultDTO<WarehouseListDTO> result = new PagedResultDTO<WarehouseListDTO>(page, RowCount, PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<WarehouseListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }   
        public async Task<ResponseDTO<List<WarehouseListDTO>?>> ListAll()
        {
            try
            {
                List<Warehouse> list = await daoWarehouse.getListAll();
                List<WarehouseListDTO> data = mapper.Map<List<WarehouseListDTO>>(list);
                return new ResponseDTO<List<WarehouseListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID)
        {
            if (DTO.WarehouseName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên kho không được để trống", (int)HttpStatusCode.NotAcceptable);
            }
            try
            {
                Warehouse ware = new Warehouse()
                {
                    WarehouseName = DTO.WarehouseName.Trim(),
                    WarehouseKeeperid = DTO.WarehouseKeeperId,
                    WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim(),
                    CreatorId = CreatorID,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                await daoWarehouse.CreateWarehouse(ware);
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO)
        {
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
                // if not found
                if (ware == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                if (DTO.WarehouseName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên kho không được để trống", (int)HttpStatusCode.Conflict);
                }
                ware.WarehouseName = DTO.WarehouseName.Trim();
                ware.WarehouseKeeperid = DTO.WarehouseKeeperId;
                ware.WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim();
                ware.UpdateAt = DateTime.Now;
                await daoWarehouse.UpdateWarehouse(ware);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(int WarehouseID)
        {
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
                // if not found
                if (ware == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                await daoWarehouse.DeleteWarehouse(ware);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
