using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;

namespace API.Services
{
    public class OtherMaterialsService
    {
        private readonly DAOOtherMaterials daoOtherMaterials = new DAOOtherMaterials();
        private readonly DAOOtherMaterialsCategory daoCategory = new DAOOtherMaterialsCategory();
        private readonly DAOSupplier daoSupplier = new DAOSupplier();
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();
        private async Task<List<OtherMaterialsListDTO>> getList(string? filter, int page)
        {
            List<OtherMaterial> list = await daoOtherMaterials.getList(filter, page);
            List<OtherMaterialsListDTO> result = new List<OtherMaterialsListDTO>();
            foreach (OtherMaterial item in list)
            {
                OtherMaterialsCategory? category = await daoCategory.getOtherMaterialsCategory(item.OtherMaterialsCategoryId);
                Supplier? supplier = await daoSupplier.getSupplier(item.SupplierId);
                Warehouse? ware = await daoWarehouse.getWarehouse(item.WarehouseId);
                if (category != null && supplier != null && ware != null)
                {
                    OtherMaterialsListDTO DTO = new OtherMaterialsListDTO()
                    {
                        OtherMaterialsId = item.OtherMaterialsId,
                        Unit = item.Unit,
                        Quantity = item.Quantity,
                        Code = item.Code,
                        SupplierName = supplier.SupplierName,
                        WarehouseName = ware.WarehouseName,
                        OtherMaterialsCategoryName = category.OtherMaterialsCategoryName
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }
        public async Task<PagedResultDTO<OtherMaterialsListDTO>> List(string? filter, int page)
        {
            List<OtherMaterialsListDTO> list = await getList(filter, page);
            int RowCount = await daoOtherMaterials.getRowCount(filter);
            int sum = await daoOtherMaterials.getSum(filter);
            return new PagedResultDTO<OtherMaterialsListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE, list, sum);
        }
    }
}
