using DataAccess.DTO.WarehouseDTO;
using DataAccess.DTO;
using DataAccess.Model.DAO;
using System.ComponentModel;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;

namespace API.Services
{
    public class WarehouseService
    {
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();
        private async Task<List<WarehouseListDTO>> getList(string? name, int page)
        {
            List<Warehouse> list = await daoWarehouse.getList(name, page);
            List<WarehouseListDTO> result = new List<WarehouseListDTO>();
            foreach (Warehouse item in list)
            {
                WarehouseListDTO DTO = new WarehouseListDTO()
                {
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.WarehouseName,
                    WarehouseKeeperId = item.WarehouseKeeperId,
                    WarehouseAddress = item.WarehouseAddress
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<WarehouseListDTO>> List(string? name, int page)
        {
            List<WarehouseListDTO> list = await getList(name, page);
            int RowCount = await daoWarehouse.getRowCount(name);
            return new PagedResultDTO<WarehouseListDTO>(page, RowCount,PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE, list);
        }

        public async Task<ResponseDTO<bool>> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID)
        {
            // if warehouse already exist
            if(await daoWarehouse.isExist(DTO.WarehouseName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Kho đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            Warehouse ware = new Warehouse()
            {
                WarehouseName = DTO.WarehouseName.Trim(),
                WarehouseKeeperId = DTO.WarehouseKeeperId,
                WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim(),
                CreatorId = CreatorID,
                CreatedAt = DateTime.Now,
                UpdateAt = null,
                IsDeleted = false,
            };
            int number = await daoWarehouse.CreateWarehouse(ware);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO)
        {
            Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
            // if not found
            if(ware == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy kho", (int) HttpStatusCode.NotFound);
            }

            // if ware exist
            if(await daoWarehouse.isExist(WarehouseID, DTO.WarehouseName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Kho đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            ware.WarehouseName = DTO.WarehouseName.Trim();
            ware.WarehouseKeeperId = DTO.WarehouseKeeperId;
            ware.WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim();
            ware.UpdateAt = DateTime.Now;
            int number = await daoWarehouse.UpdateWarehouse(ware);
            // if update successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(int WarehouseID)
        {
            Warehouse? ware = await daoWarehouse.getWarehouse(WarehouseID);
            // if not found
            if (ware == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy kho", (int) HttpStatusCode.NotFound);
            }
            int number = await daoWarehouse.DeleteWarehouse(ware);
            // if delete successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
