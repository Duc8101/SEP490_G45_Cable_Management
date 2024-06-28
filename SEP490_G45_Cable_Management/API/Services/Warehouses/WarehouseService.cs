using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.WarehouseDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Warehouses
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();

        public WarehouseService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<WarehouseListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<Warehouse> list = await daoWarehouse.getListPaged(name, page);
                List<WarehouseListDTO> DTOs = _mapper.Map<List<WarehouseListDTO>>(list);
                int RowCount = await daoWarehouse.getRowCount(name);
                Pagination<WarehouseListDTO> result = new Pagination<WarehouseListDTO>(page, RowCount, PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<WarehouseListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<WarehouseListDTO>?>> ListAll()
        {
            try
            {
                List<Warehouse> list = await daoWarehouse.getListAll();
                List<WarehouseListDTO> data = _mapper.Map<List<WarehouseListDTO>>(list);
                return new ResponseBase<List<WarehouseListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID)
        {
            if (DTO.WarehouseName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên kho không được để trống", (int)HttpStatusCode.NotAcceptable);
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
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO)
        {
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
                // if not found
                if (ware == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                if (DTO.WarehouseName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên kho không được để trống", (int)HttpStatusCode.Conflict);
                }
                ware.WarehouseName = DTO.WarehouseName.Trim();
                ware.WarehouseKeeperid = DTO.WarehouseKeeperId;
                ware.WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim();
                ware.UpdateAt = DateTime.Now;
                await daoWarehouse.UpdateWarehouse(ware);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(int WarehouseID)
        {
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
                // if not found
                if (ware == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                await daoWarehouse.DeleteWarehouse(ware);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
