using DataAccess.DTO.WarehouseDTO;
using DataAccess.DTO;
using DataAccess.Model.DAO;
using System.ComponentModel;
using DataAccess.Entity;
using DataAccess.Const;

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
                    WarehouseAddress = item.WarehouseAddress,
                    CreatorId = item.CreatorId,
                    CreatedAt = item.CreatedAt,
                    UpdateAt = item.UpdateAt
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
    }
}
