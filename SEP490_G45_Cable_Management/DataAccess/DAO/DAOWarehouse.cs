using Common.Const;
using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOWarehouse : BaseDAO
    {
        private IQueryable<Warehouse> getQuery(string? name)
        {
            IQueryable<Warehouse> query = context.Warehouses.Include(w => w.WarehouseKeeper).Where(w => w.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
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
        public async Task CreateWarehouse(Warehouse ware)
        {
            await context.Warehouses.AddAsync(ware);
            await context.SaveChangesAsync();
        }
        public async Task<Warehouse?> getWarehouse(int ID)
        {
            return await context.Warehouses.SingleOrDefaultAsync(w => w.WarehouseId == ID && w.IsDeleted == false);
        }
        public async Task UpdateWarehouse(Warehouse ware)
        {
            context.Warehouses.Update(ware);
            await context.SaveChangesAsync();
        }
        public async Task DeleteWarehouse(Warehouse ware)
        {
            ware.IsDeleted = true;
            await UpdateWarehouse(ware);
        }

    }
}
