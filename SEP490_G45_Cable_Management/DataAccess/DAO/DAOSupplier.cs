using Common.Const;
using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOSupplier : BaseDAO
    {
        public DAOSupplier(CableManagementContext context) : base(context)
        {
        }

        public void CreateSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            Save();
        }

        public bool isExist(string supplierName)
        {
            return _context.Suppliers.Any(s => s.SupplierName == supplierName.Trim() && s.IsDeleted == false);
        }

        private IQueryable<Supplier> getQuery(string? name)
        {
            IQueryable<Supplier> query = _context.Suppliers.Where(s => s.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(s => s.SupplierName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }

        public List<Supplier> getListSupplier(string? name, int page)
        {
            IQueryable<Supplier> query = getQuery(name);
            return query.OrderByDescending(u => u.UpdateAt).Skip((page - 1) * (int) PageSize.Size)
                .Take((int)PageSize.Size).ToList();
        }

        public int getRowCount(string? name)
        {
            IQueryable<Supplier> query = getQuery(name);
            return query.Count();
        }

        public List<Supplier> getListSupplier()
        {
            IQueryable<Supplier> query = getQuery(null);
            return query.OrderByDescending(c => c.UpdateAt).ToList();
        }

        public Supplier? getSupplier(int supplierId)
        {
            return _context.Suppliers.SingleOrDefault(s => s.SupplierId == supplierId && s.IsDeleted == false);
        }

        public bool isExist(int supplierId, string supplierName)
        {
            return _context.Suppliers.Any(s => s.SupplierName == supplierName.Trim() && s.SupplierId != supplierId && s.IsDeleted == false);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            Save();
        }

        public void DeleteSupplier(Supplier supplier)
        {
            supplier.IsDeleted = true;
            UpdateSupplier(supplier);
        }
    }
}
