using DataAccess.Const;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOOtherMaterial : BaseDAO
    {
        private IQueryable<OtherMaterial> getQuery(string? filter,int? WareHouseID, Guid? WareHouseKeeperID)
        {
            IQueryable<OtherMaterial> query = context.OtherMaterials.Include(o => o.OtherMaterialsCategory).Include(o => o.Supplier).Include(o => o.Warehouse)
                .Where(o => o.IsDeleted == false);
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(o => o.OtherMaterialsCategory.OtherMaterialsCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || (o.Warehouse != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())));
            }
            // if login as warehousekeeper
            if(WareHouseKeeperID != null)
            {
                query = query.Where(o => o.Warehouse != null && o.Warehouse.WarehouseKeeperid == WareHouseKeeperID);
            }
            // if choose warehouse
            if(WareHouseID != null)
            {
                query = query.Where(o => o.WarehouseId == WareHouseID);
            }
            return query;
        }
        public async Task<List<OtherMaterial>> getListPaged(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, WareHouseID, WareHouseKeeperID);
            return await query.OrderByDescending(o => o.UpdateAt).Skip(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE)
                .ToListAsync();
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
            && o.Unit == DTO.Unit.Trim() && o.Code == DTO.Code.Trim() /*&& o.SupplierId == DTO.SupplierId*/
            && o.Status == (DTO.Status == null || DTO.Status.Trim().Length == 0 ? null : DTO.Status.Trim()) && o.WarehouseId == DTO.WarehouseId).FirstOrDefaultAsync();
        }

        public void CreateMaterial(OtherMaterial material)
        {
            context.OtherMaterials.Add(material);
            context.SaveChanges();
        }

        public void UpdateMaterial(OtherMaterial material)
        {
            context.OtherMaterials.Update(material);
            context.SaveChanges();
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

        public async Task<bool> isExist(OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.AnyAsync(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && o.Code == DTO.Code.Trim() && o.Status == DTO.Status.Trim()
            && o.WarehouseId == DTO.WarehouseId);
        }

        public async Task<bool> isExist(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.AnyAsync(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && o.Code == DTO.Code.Trim() && o.Status == DTO.Status.Trim()
            && o.WarehouseId == DTO.WarehouseId && o.OtherMaterialsId != OtherMaterialsID);
        }

        public void DeleteMaterial(OtherMaterial material)
        {
            material.IsDeleted = true;
            UpdateMaterial(material);
        }

        public async Task<List<OtherMaterialsCategory>> getListCategory(int? WarehouseID)
        {
            IQueryable<OtherMaterial> query = context.OtherMaterials.Include(o => o.OtherMaterialsCategory).Where(o => o.IsDeleted == false);
            // if choose warehouse
            if (WarehouseID != null)
            {
                query = query.Where(o => o.WarehouseId == WarehouseID);
            }
            List<OtherMaterialsCategory> result = await query.Select(o => o.OtherMaterialsCategory).Distinct().ToListAsync();
            return result;
        }

        private async Task<List<OtherMaterial>> getList(int CategoryID, int? WarehouseID)
        {
            IQueryable<OtherMaterial> query = context.OtherMaterials.Where(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == CategoryID);
            // if choose warehouse
            if (WarehouseID != null)
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
        public async Task<OtherMaterial?> getOtherMaterialCreate()
        {
            return await context.OtherMaterials.OrderByDescending(o => o.OtherMaterialsId).FirstOrDefaultAsync();
        }
    }
}
