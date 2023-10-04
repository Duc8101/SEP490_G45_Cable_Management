using DataAccess.Const;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOOtherMaterial : BaseDAO
    {
        public async Task<List<OtherMaterial>> getList(string? filter, int page)
        {
            if(filter == null || filter.Trim().Length == 0)
            {
                return await context.OtherMaterials.Where(o => o.IsDeleted == false).Skip(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE * (page - 1))
                    .Take(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE).OrderByDescending(o => o.UpdateAt).ToListAsync();
            }
            return await context.OtherMaterials.Where(o => o.IsDeleted == false 
            && ((o.OtherMaterialsCategory.OtherMaterialsCategoryName != null && o.OtherMaterialsCategory.OtherMaterialsCategoryName
            .ToLower().Contains(filter.ToLower().Trim())) || (o.Supplier != null && o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))
            || (o.Warehouse != null && o.Warehouse.WarehouseName != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim()))))
                .Skip(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE)
                .OrderByDescending(o => o.UpdateAt).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            List<OtherMaterial> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false).ToListAsync();
            }
            else
            {
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false 
                && ((o.OtherMaterialsCategory.OtherMaterialsCategoryName != null && o.OtherMaterialsCategory.OtherMaterialsCategoryName.ToLower().Contains(filter.ToLower().Trim())) 
                || (o.Supplier != null && o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))
                || (o.Warehouse != null && o.Warehouse.WarehouseName != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())))).ToListAsync();
            }
            return list.Count;
        }

        public async Task<int> getSum(string? filter)
        {
            List<OtherMaterial> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false).ToListAsync();
            }
            else
            {
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false && 
                ((o.OtherMaterialsCategory.OtherMaterialsCategoryName != null && o.OtherMaterialsCategory.OtherMaterialsCategoryName.ToLower().Contains(filter.ToLower().Trim())) 
                || (o.Supplier != null && o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))
                || (o.Warehouse != null && o.Warehouse.WarehouseName != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())))).ToListAsync();
            }
            int sum = 0;
            foreach (OtherMaterial material in list)
            {
                if(material.Quantity != null)
                {
                    sum = sum + material.Quantity.Value;
                } 
            }
            return sum;
        }

        public async Task<OtherMaterial?> getOtherMaterial(OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.Where(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && (DTO.Unit != null && o.Unit == DTO.Unit.Trim()) && (DTO.Code != null && o.Code == DTO.Code.Trim()) && o.SupplierId == DTO.SupplierId 
            && (DTO.Status != null && o.Status == DTO.Status.Trim()) && o.WarehouseId == DTO.WarehouseId).FirstOrDefaultAsync();
        }

        public async Task<int> CreateMaterial(OtherMaterial material)
        {
            await context.OtherMaterials.AddAsync(material);
            return await context.SaveChangesAsync();
        }

        public async Task<int> UpdateMaterial(OtherMaterial material)
        {
            context.OtherMaterials.Update(material);
            return await context.SaveChangesAsync();
        }

        public async Task<OtherMaterial?> getOtherMaterial(int ID)
        {
            return await context.OtherMaterials.SingleOrDefaultAsync(o => o.OtherMaterialsId == ID && o.IsDeleted == false);
        }

        public async Task<bool> isExist(OtherMaterialsCreateUpdateDTO DTO)
        {
            return await context.OtherMaterials.AnyAsync(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && (DTO.Unit != null && o.Unit == DTO.Unit.Trim()) && (DTO.Code != null && o.Code == DTO.Code.Trim()) && o.SupplierId == DTO.SupplierId && (DTO.Status != null && o.Status == DTO.Status.Trim())
            && o.WarehouseId == DTO.WarehouseId);
        }

        public async Task<int> DeleteMaterial(OtherMaterial material)
        {
            material.IsDeleted = true;
            return await UpdateMaterial(material);
        }
    }
}
