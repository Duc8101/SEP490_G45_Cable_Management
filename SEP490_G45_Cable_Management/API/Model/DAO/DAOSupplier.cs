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
    public class DAOSupplier : BaseDAO
    {
        public void CreateSupplier(Supplier supplier)
        {
            context.Suppliers.Add(supplier);
            context.SaveChanges();
        }

        public bool isExist(string SupplierName)
        {
            return context.Suppliers.Any(s => s.SupplierName == SupplierName.Trim() && s.IsDeleted == false);
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

        public async Task<List<Supplier>> getListPaged(string? name, int page)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.Skip((page - 1) * PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).Take(PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? name)
        {
            IQueryable<Supplier> query = getQuery(name);
            return await query.CountAsync();
        }

        public async Task<List<Supplier>> getListAll()
        {
            IQueryable<Supplier> query = getQuery(null);
            return await query.ToListAsync();
        }

        public async Task<Supplier?> getSupplier(int SupplierID)
        {
            return await context.Suppliers.SingleOrDefaultAsync(s => s.SupplierId == SupplierID && s.IsDeleted == false);
        }

        public bool isExist(int SupplierID, string SupplierName)
        {
            return context.Suppliers.Any(s => s.SupplierName == SupplierName.Trim() && s.SupplierId != SupplierID && s.IsDeleted == false);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            context.Suppliers.Update(supplier);
            context.SaveChanges();
        }

        public void DeleteSupplier(Supplier supplier)
        {
            supplier.IsDeleted = true;
            UpdateSupplier(supplier);
        }
    }
}
