using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API.Model.DAO
{
    public class DAOWarehouse : BaseDAO
    {
        private IQueryable<Warehouse> getQuery(string? name)
        {
            IQueryable<Warehouse> query = context.Warehouses.Include(w => w.WarehouseKeeper).Where(w => w.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(w => w.WarehouseName != null && w.WarehouseName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }
        public async Task<List<Warehouse>> getListPaged(string? name, int page)
        {
            IQueryable<Warehouse> query = getQuery(name);
            return await query.OrderByDescending(w => w.UpdateAt).Skip(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name)
        {
            IQueryable<Warehouse> query = getQuery(name);
            return await query.CountAsync();
        }
        public async Task<List<Warehouse>> getListAll()
        {
            IQueryable<Warehouse> query = getQuery(null);
            return await query.OrderByDescending(w => w.UpdateAt).ToListAsync();
        }
        public bool isExist(string name)
        {
            return context.Warehouses.Any(w => w.WarehouseName == name.Trim());
        }

        public void CreateWarehouse(Warehouse ware)
        {
            context.Warehouses.Add(ware);
            context.SaveChanges();
        }

        public async Task<Warehouse?> getWarehouse(int ID)
        {
            return await context.Warehouses.SingleOrDefaultAsync(w => w.WarehouseId == ID && w.IsDeleted == false);
        }

        public bool isExist(int ID, string name)
        {
            return context.Warehouses.Any(w => w.WarehouseName == name.Trim() && w.WarehouseId != ID);
        }

        public void UpdateWarehouse(Warehouse ware)
        {
            context.Warehouses.Update(ware);
            context.SaveChanges();
        }

        public void DeleteWarehouse(Warehouse ware)
        {
            ware.IsDeleted = true;
            UpdateWarehouse(ware);
        }

    }
}
