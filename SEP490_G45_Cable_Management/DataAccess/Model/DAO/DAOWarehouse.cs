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
    public class DAOWarehouse : BaseDAO
    {
        public async Task<List<Warehouse>> getList(string? name, int page)
        {
            if (name == null || name.Trim().Length == 0)
            {
                return await context.Warehouses.Where(w => w.IsDeleted == false).Skip(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE).OrderByDescending(w => w.UpdateAt).ToListAsync();
            }
            return await context.Warehouses.Where(w => w.IsDeleted == false && w.WarehouseName.ToLower().Contains(name.ToLower().Trim())).Skip(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE).OrderByDescending(w => w.UpdateAt).ToListAsync();
        }

        public async Task<int> getRowCount(string? name)
        {
            List<Warehouse> list;
            if (name == null || name.Trim().Length == 0)
            {
                list = await context.Warehouses.Where(w => w.IsDeleted == false).ToListAsync();
            }
            else
            {
                list = await context.Warehouses.Where(w => w.IsDeleted == false && w.WarehouseName.ToLower().Contains(name.ToLower().Trim())).ToListAsync();
            }
            return list.Count;
        }
    }
}
