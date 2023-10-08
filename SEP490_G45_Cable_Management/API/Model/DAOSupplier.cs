using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model
{
    public class DAOSupplier : BaseDAO
    {
        public async Task<int> CreateSupplier(Supplier supplier)
        {
            await context.Suppliers.AddAsync(supplier);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> isExist(string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName == SupplierName.Trim());
        }

        private IQueryable<Supplier> getQuery(string? name)
        {
            IQueryable<Supplier> query = context.Suppliers.Where(s => s.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(s => s.SupplierName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }

        public async Task<List<Supplier>> getList(string? name, int page)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.Skip((page - 1) * PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).Take(PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? name)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.CountAsync();
        }

        public async Task<Supplier?> getSupplier(int SupplierID)
        {
            return await context.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == SupplierID && s.IsDeleted == false);
        }

        public async Task<bool> isExist(int SupplierID, string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName == SupplierName.Trim() && s.SupplierId != SupplierID);
        }

        public async Task<int> UpdateSupplier(Supplier supplier)
        {
            context.Suppliers.Update(supplier);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteSupplier(Supplier supplier)
        {
            supplier.IsDeleted = true;
            return await UpdateSupplier(supplier);
        }
    }
}
