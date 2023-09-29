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

        public async Task<bool> isExist(string SupplierName)
        {
            return await context.Suppliers.AnyAsync(s => s.SupplierName == SupplierName.Trim());
        }

        public async Task<List<Supplier>> getList(string? name, int page)
        {
            List<Supplier> list;
            if (name == null || name.Trim().Length == 0)
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false).OrderByDescending(x => x.UpdateAt).Skip((page - 1) * PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).Take(PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).ToListAsync();
            }
            else
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false && s.SupplierName.ToLower().Contains(name.ToLower().Trim())).OrderByDescending(x => x.UpdateAt).Skip((page - 1) * PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).Take(PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).ToListAsync();
            }
            return list;
        }

        public async Task<int> getRowCount(string? name)
        {
            List<Supplier> list;
            if (name == null || name.Trim().Length == 0)
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false).OrderByDescending(x => x.UpdateAt).ToListAsync();
            }
            else
            {
                list = await context.Suppliers.Where(s => s.IsDeleted == false && s.SupplierName.ToLower().Contains(name.ToLower().Trim())).OrderByDescending(x => x.UpdateAt).ToListAsync();
            }
            return list.Count;
        }

        public async Task<Supplier?> getSupplier(int SupplierID)
        {
            return await context.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == SupplierID);
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
