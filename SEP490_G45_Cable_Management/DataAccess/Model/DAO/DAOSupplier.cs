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
    public class DAOSupplier : BaseDAO
    {
        public async Task<int> CreateSupplier(Supplier supplier)
        {
            await context.Suppliers.AddAsync(supplier);
            return await context.SaveChangesAsync();
        }

        public async Task<Supplier?> getSupplier(string name)
        {
            return await context.Suppliers.FirstOrDefaultAsync(x => x.SupplierName.ToLower().Trim() == name.ToLower().Trim()
               && x.IsDeleted == false);
        }

        public async Task<List<Supplier>> getList(string? filter, int page)
        {
            List<Supplier> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false).OrderByDescending(x => x.UpdateAt).Skip((page - 1) * PageSizeConst.MAX_PAGE_SIZE_SUPPLIER_LIST).Take(PageSizeConst.MAX_PAGE_SIZE_SUPPLIER_LIST).ToListAsync();
            }
            else
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false && s.SupplierName.ToLower().Contains(filter.ToLower().Trim())).OrderByDescending(x => x.UpdateAt).Skip((page - 1) * PageSizeConst.MAX_PAGE_SIZE_SUPPLIER_LIST).Take(PageSizeConst.MAX_PAGE_SIZE_SUPPLIER_LIST).ToListAsync();
            }
            return list;
        }

        public async Task<Supplier?> getSupplier(int SupplierID)
        {
            return await context.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == SupplierID);
        }

        public async Task<bool> isSupplierExist(int SupplierID, string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName.ToLower() == SupplierName.ToLower().Trim() && s.SupplierId != SupplierID);
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
