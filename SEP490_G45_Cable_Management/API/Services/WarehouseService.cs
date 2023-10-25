using DataAccess.DTO.WarehouseDTO;
using DataAccess.DTO;
using System.ComponentModel;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using API.Model.DAO;

namespace API.Services
{
    public class WarehouseService
    {
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();
        private async Task<List<WarehouseListDTO>> getListPaged(string? name, int page)
        {
            List<Warehouse> list = await daoWarehouse.getListPaged(name, page);
            List<WarehouseListDTO> result = new List<WarehouseListDTO>();
            foreach (Warehouse item in list)
            {
                WarehouseListDTO DTO = new WarehouseListDTO()
                {
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.WarehouseName,
                    WarehouseKeeperId = item.WarehouseKeeperid,
                    WareWarehouseKeeperName = item.WarehouseKeeper == null ? null : item.WarehouseKeeper.Lastname + " " + item.WarehouseKeeper.Firstname,
                    WarehouseAddress = item.WarehouseAddress
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<WarehouseListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<WarehouseListDTO> list = await getListPaged(name, page);
                int RowCount = await daoWarehouse.getRowCount(name);
                PagedResultDTO<WarehouseListDTO> result = new PagedResultDTO<WarehouseListDTO>(page, RowCount, PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<WarehouseListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
        private async Task<List<WarehouseListDTO>> getListAll()
        {
            List<Warehouse> list = await daoWarehouse.getListAll();
            List<WarehouseListDTO> result = new List<WarehouseListDTO>();
            foreach (Warehouse item in list)
            {
                WarehouseListDTO DTO = new WarehouseListDTO()
                {
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.WarehouseName,
                    WarehouseKeeperId = item.WarehouseKeeperid,
                    WareWarehouseKeeperName = item.WarehouseKeeper == null ? null : item.WarehouseKeeper.Lastname + " " + item.WarehouseKeeper.Firstname,
                    WarehouseAddress = item.WarehouseAddress
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<List<WarehouseListDTO>?>> ListAll()
        {
            try
            {
                List<WarehouseListDTO> list = await getListAll();
                return new ResponseDTO<List<WarehouseListDTO>?>(list, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<WarehouseListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID)
        {
            if(DTO.WarehouseName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên kho không được để trống", (int) HttpStatusCode.NotAcceptable);
            }
            try
            {
               /* // if warehouse already exist
                if (await daoWarehouse.isExist(DTO.WarehouseName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Kho đã tồn tại", (int) HttpStatusCode.Conflict);
                }*/
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
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
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
                /*// if ware exist
                if (await daoWarehouse.isExist(WarehouseID, DTO.WarehouseName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Kho đã tồn tại", (int) HttpStatusCode.NotAcceptable);
                }*/
                ware.WarehouseName = DTO.WarehouseName.Trim();
                ware.WarehouseKeeperid = DTO.WarehouseKeeperId;
                ware.WarehouseAddress = DTO.WarehouseAddress == null || DTO.WarehouseAddress.Trim().Length == 0 ? null : DTO.WarehouseAddress.Trim();
                ware.UpdateAt = DateTime.Now;
                await daoWarehouse.UpdateWarehouse(ware);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
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
                    return new ResponseDTO<bool>(false, "Không tìm thấy kho", (int) HttpStatusCode.NotFound);
                }
                await daoWarehouse.DeleteWarehouse(ware);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
