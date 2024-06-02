using DataAccess.Const;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAOOtherMaterial : BaseDAO
    {
        private IQueryable<OtherMaterial> getQuery(string? filter, int? WareHouseID, Guid? WareHouseKeeperID)
        {
            IQueryable<OtherMaterial> query = context.OtherMaterials.Include(o => o.OtherMaterialsCategory).Include(o => o.Supplier).Include(o => o.Warehouse)
                .Where(o => o.IsDeleted == false);
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(o => o.OtherMaterialsCategory.OtherMaterialsCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || (o.Warehouse != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())));
            }
            // if login as warehouse keeper
            if (WareHouseKeeperID.HasValue)
            {
                query = query.Where(o => o.Warehouse != null && o.Warehouse.WarehouseKeeperid == WareHouseKeeperID);
            }
            // if choose warehouse
            if (WareHouseID.HasValue)
            {
                query = query.Where(o => o.WarehouseId == WareHouseID);
            }
            return query;
        }
        public async Task<List<OtherMaterial>> getListPaged(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, WareHouseID, WareHouseKeeperID);
            return await query.OrderByDescending(o => o.UpdateAt).Skip(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE * (page - 1)).ToListAsync();
        }
        public async Task<List<OtherMaterial>> getListAll(int? WareHouseID)
        {
            IQueryable<OtherMaterial> query = getQuery(null, WareHouseID, null);
            return await query.OrderByDescending(o => o.UpdateAt).ToListAsync();
        }
        public async Task<int> getRowCount(string? filter, int? WareHouseID, Guid? WareHouseKeeperID)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, WareHouseID, WareHouseKeeperID);
            return await query.CountAsync();
        }
        public async Task<int> getSum(string? filter, int? WareHouseID, Guid? WareHouseKeeperID)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, WareHouseID, WareHouseKeeperID);
            List<OtherMaterial> list = await query.ToListAsync();
            int sum = 0;
            foreach (OtherMaterial material in list)
            {
                sum = sum + material.Quantity;
            }
            return sum;
        }
        public async Task<OtherMaterial?> getOtherMaterial(OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.Where(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.SupplierId == 1
            && o.Status == DTO.Status.Trim() && o.WarehouseId == DTO.WarehouseId).FirstOrDefaultAsync();
        }
        public async Task<int> CreateMaterial(OtherMaterial material)
        {
            await context.OtherMaterials.AddAsync(material);
            await context.SaveChangesAsync();
            int MaterialID = await context.OtherMaterials.MaxAsync(m => m.OtherMaterialsId);
            return MaterialID;
        }
        public async Task UpdateMaterial(OtherMaterial material)
        {
            context.OtherMaterials.Update(material);
            await context.SaveChangesAsync();
        }
        public async Task<OtherMaterial?> getOtherMaterial(int ID)
        {
            return await context.OtherMaterials.SingleOrDefaultAsync(o => o.OtherMaterialsId == ID && o.IsDeleted == false);
        }
        public async Task<List<OtherMaterial>> getListAll(int CategoryID)
        {
            return await context.OtherMaterials.Include(o => o.OtherMaterialsCategory)
                .Where(o => o.OtherMaterialsCategoryId == CategoryID).ToListAsync();
        }
        public async Task<bool> isExist(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.AnyAsync(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.Status == DTO.Status.Trim()
            && o.WarehouseId == DTO.WarehouseId && o.OtherMaterialsId != OtherMaterialsID);
        }
        public async Task DeleteMaterial(OtherMaterial material)
        {
            material.IsDeleted = true;
            await UpdateMaterial(material);
        }
        public async Task<List<OtherMaterialsCategory>> getListCategory(int? WarehouseID)
        {
            IQueryable<OtherMaterial> query = getQuery(null, WarehouseID, null);
            List<OtherMaterialsCategory> result = await query.Select(o => o.OtherMaterialsCategory).Distinct().ToListAsync();
            return result;
        }
        private async Task<List<OtherMaterial>> getList(int CategoryID, int? WarehouseID)
        {
            IQueryable<OtherMaterial> query = context.OtherMaterials.Where(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == CategoryID);
            // if choose warehouse
            if (WarehouseID.HasValue)
            {
                query = query.Where(o => o.WarehouseId == WarehouseID);
            }
            return await query.ToListAsync();
        }
        public async Task<int> getSum(int CategoryID, int? WarehouseID)
        {
            List<OtherMaterial> list = await getList(CategoryID, WarehouseID);
            int sum = 0;
            foreach (OtherMaterial item in list)
            {
                sum = sum + item.Quantity;
            }
            return sum;
        }
        public async Task<OtherMaterial?> getOtherMaterial(int? WareHouseID, string? code, string status, string unit)
        {
            return await context.OtherMaterials.Where(o => o.WarehouseId == WareHouseID && o.Code == code && o.Status == status && o.Unit == unit && o.IsDeleted == false).FirstOrDefaultAsync();
        }
        public async Task<OtherMaterial?> getOtherMaterial(OtherMaterialsCancelOutsideDTO DTO)
        {
            return await context.OtherMaterials.Include(x => x.OtherMaterialsCategory).Where(o => o.Unit == DTO.Unit && o.Status == DTO.Status
            && o.SupplierId == DTO.SupplierId && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId && o.IsDeleted).FirstOrDefaultAsync();
        }
        public async Task<OtherMaterial?> getOtherMaterial(OtherMaterialsRecoveryDTO DTO)
        {
            return await context.OtherMaterials.Where(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.SupplierId == DTO.SupplierId
            && o.Status == DTO.Status.Trim() && o.WarehouseId == DTO.WarehouseId).FirstOrDefaultAsync();
        }

    }
}
