using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOWarehouse : BaseDAO
    {
        public DAOWarehouse(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<Warehouse> getQuery(string? name)
        {
            IQueryable<Warehouse> query = _context.Warehouses.Include(w => w.WarehouseKeeper).Where(w => w.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(w => w.WarehouseName != null && w.WarehouseName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }

        public List<Warehouse> getListWarehouse(string? name, int page)
        {
            IQueryable<Warehouse> query = getQuery(name);
            return query.OrderByDescending(w => w.UpdateAt).Skip((int) PageSize.Size * (page - 1)).Take((int)PageSize.Size)
                .ToList();
        }

        public int getRowCount(string? name)
        {
            IQueryable<Warehouse> query = getQuery(name);
            return query.Count();
        }

        public List<Warehouse> getListWarehouse()
        {
            IQueryable<Warehouse> query = getQuery(null);
            return query.OrderByDescending(w => w.UpdateAt).ToList();
        }

        public void CreateWarehouse(Warehouse ware)
        {
            _context.Warehouses.Add(ware);
            Save();
        }

        public Warehouse? getWarehouse(int id)
        {
            return _context.Warehouses.SingleOrDefault(w => w.WarehouseId == id && w.IsDeleted == false);
        }

        public void UpdateWarehouse(Warehouse ware)
        {
            _context.Warehouses.Update(ware);
            Save();
        }

        public void DeleteWarehouse(Warehouse ware)
        {
            ware.IsDeleted = true;
            UpdateWarehouse(ware);
        }

    }
}
