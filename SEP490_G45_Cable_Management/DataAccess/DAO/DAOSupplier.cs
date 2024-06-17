using Common.Const;
using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOSupplier : BaseDAO
    {
        public async Task CreateSupplier(Supplier supplier)
        {
            await context.Suppliers.AddAsync(supplier);
            await context.SaveChangesAsync();
        }
        public async Task<bool> isExist(string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName == SupplierName.Trim() && s.IsDeleted == false);
        }
        private IQueryable<Supplier> getQuery(string? name)
        {
            IQueryable<Supplier> query = context.Suppliers.Where(s => s.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(s => s.SupplierName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }
        public async Task<List<Supplier>> getListPaged(string? name, int page)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.OrderByDescending(u => u.UpdateAt).Skip((page - 1) * PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE)
                .Take(PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.CountAsync();
        }
        public async Task<List<Supplier>> getListAll()
        {
            IQueryable<Supplier> query = getQuery(null);
            return await query.OrderByDescending(c => c.UpdateAt).ToListAsync();
        }
        public async Task<Supplier?> getSupplier(int SupplierID)
        {
            return await context.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == SupplierID && s.IsDeleted == false);
        }
        public async Task<bool> isExist(int SupplierID, string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName == SupplierName.Trim() && s.SupplierId != SupplierID && s.IsDeleted == false);
        }
        public async Task UpdateSupplier(Supplier supplier)
        {
            context.Suppliers.Update(supplier);
            await context.SaveChangesAsync();
        }
        public async Task DeleteSupplier(Supplier supplier)
        {
            supplier.IsDeleted = true;
            await UpdateSupplier(supplier);
        }
    }
}
