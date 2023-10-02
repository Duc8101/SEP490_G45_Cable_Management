using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOOtherMaterials : BaseDAO
    {
        public async Task<List<OtherMaterial>> getList(string? filter, int page)
        {
            if(filter == null || filter.Trim().Length == 0)
            {
                return await context.OtherMaterials.Where(o => o.IsDeleted == false).Skip(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE * (page - 1))
                    .Take(PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE).OrderByDescending(o => o.UpdateAt).ToListAsync();
            }
            return await context.OtherMaterials.Where(o => o.IsDeleted == false && (o.OtherMaterialsCategory.OtherMaterialsCategoryName
            .ToLower().Contains(filter.ToLower().Trim()) || o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())
            || o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())))
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
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false && (o.OtherMaterialsCategory.OtherMaterialsCategoryName
                .ToLower().Contains(filter.ToLower().Trim()) || o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())
                || o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim()))).ToListAsync();
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
                list = await context.OtherMaterials.Where(o => o.IsDeleted == false && (o.OtherMaterialsCategory.OtherMaterialsCategoryName
                .ToLower().Contains(filter.ToLower().Trim()) || o.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())
                || o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim()))).ToListAsync();
            }
            int sum = 0;
            foreach (OtherMaterial material in list)
            {
                sum = sum + material.Quantity;
            }
            return sum;
        }
    }
}
