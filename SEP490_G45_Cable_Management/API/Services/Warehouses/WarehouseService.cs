using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.WarehouseDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Warehouses
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        private readonly DAOWarehouse _daoWarehouse;
        public WarehouseService(IMapper mapper, DAOWarehouse daoWarehouse) : base(mapper)
        {
            _daoWarehouse = daoWarehouse;
        }

        public ResponseBase Create(WarehouseCreateUpdateDTO DTO, Guid creatorId)
        {
            if (DTO.WarehouseName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên kho không được để trống", (int)HttpStatusCode.NotAcceptable);
            }
            try
            {
                Warehouse ware = new Warehouse()
                {
                    WarehouseName = DTO.WarehouseName.Trim(),
                    WarehouseKeeperid = DTO.WarehouseKeeperId,
                    WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim(),
                    CreatorId = creatorId,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                };
                _daoWarehouse.CreateWarehouse(ware);
                return new ResponseBase(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(int warehouseId)
        {
            try
            {
                Warehouse? ware = _daoWarehouse.getWarehouse(warehouseId);
                // if not found
                if (ware == null)
                {
                    return new ResponseBase(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                _daoWarehouse.DeleteWarehouse(ware);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListAll()
        {
            try
            {
                List<Warehouse> list = _daoWarehouse.getListWarehouse();
                List<WarehouseListDTO> data = _mapper.Map<List<WarehouseListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPaged(string? name, int page)
        {
            try
            {
                List<Warehouse> list = _daoWarehouse.getListWarehouse(name, page);
                List<WarehouseListDTO> DTOs = _mapper.Map<List<WarehouseListDTO>>(list);
                int rowCount = _daoWarehouse.getRowCount(name);
                Pagination<WarehouseListDTO> data = new Pagination<WarehouseListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    List = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(int warehouseId, WarehouseCreateUpdateDTO DTO)
        {
            try
            {
                Warehouse? ware = _daoWarehouse.getWarehouse(warehouseId);
                // if not found
                if (ware == null)
                {
                    return new ResponseBase(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound);
                }
                if (DTO.WarehouseName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên kho không được để trống", (int)HttpStatusCode.Conflict);
                }
                ware.WarehouseName = DTO.WarehouseName.Trim();
                ware.WarehouseKeeperid = DTO.WarehouseKeeperId;
                ware.WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim();
                ware.UpdateAt = DateTime.Now;
                _daoWarehouse.UpdateWarehouse(ware);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
