using Common.DTO.CableDTO;
using Common.Entity;
using Common.Enum;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOCable : BaseDAO
    {
        public DAOCable(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<Cable> getQuery(string? filter, int? warehouseId, bool isExportedToUse)
        {
            IQueryable<Cable> query = _context.Cables.Include(c => c.CableCategory).Include(c => c.Supplier).Include(c => c.Warehouse)
                .Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == isExportedToUse);
            // if search base on name
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(c => c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || c.Code.ToLower().Contains(filter.ToLower().Trim()) || c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()));
            }
            // if choose warehouse
            if (warehouseId.HasValue)
            {
                query = query.Where(c => c.WarehouseId == warehouseId);
            }
            return query;
        }
        public List<Cable> getListCable(string? filter, int? warehouseId, bool isExportedToUse, int page)
        {
            IQueryable<Cable> query = getQuery(filter, warehouseId, isExportedToUse);
            return query.OrderByDescending(c => c.UpdateAt).Skip((int) PageSize.Size * (page - 1))
                .Take((int)PageSize.Size).ToList();
        }

        public List<Cable> getListCable(int? warehouseId)
        {
            IQueryable<Cable> query = getQuery(null, warehouseId, false);
            return query.OrderByDescending(c => c.UpdateAt).ToList();
        }

        public int getRowCount(string? filter, int? warehouseId, bool isExportedToUse)
        {
            IQueryable<Cable> query = getQuery(filter, warehouseId, isExportedToUse);
            return query.Count();
        }

        public int getSum(string? filter, int? warehouseId, bool isExportedToUse)
        {
            IQueryable<Cable> query = getQuery(filter, warehouseId, isExportedToUse);
            return query.Sum(c => c.Length);
        }

        public bool isExist(CableCreateUpdateDTO DTO)
        {
            return _context.Cables.Any(c => c.IsDeleted == false && c.Code == DTO.Code.Trim() && c.Status == DTO.Status.Trim()
            && c.SupplierId == DTO.SupplierId && c.StartPoint == DTO.StartPoint && c.EndPoint == DTO.EndPoint
            && c.YearOfManufacture == DTO.YearOfManufacture);
        }

        public void CreateCable(Cable cable)
        {
            _context.Cables.Add(cable);
            Save();
        }

        public Cable? getCable(Guid cableId)
        {
            return _context.Cables.Include(c => c.CableCategory).SingleOrDefault(c => c.IsDeleted == false 
            && c.CableId == cableId);
        }

        public bool isExist(Guid cableId, CableCreateUpdateDTO DTO)
        {
            return _context.Cables.Any(c => c.IsDeleted == false && c.Code == DTO.Code.Trim() && c.Status == DTO.Status.Trim()
            && c.SupplierId == DTO.SupplierId && c.StartPoint == DTO.StartPoint && c.EndPoint == DTO.EndPoint
            && c.YearOfManufacture == DTO.YearOfManufacture && c.CableId != cableId);
        }

        public void UpdateCable(Cable cable)
        {
            _context.Cables.Update(cable);
            Save();
        }

        public void DeleteCable(Cable cable)
        {
            cable.IsDeleted = true;
            UpdateCable(cable);
        }

        public Cable? getCable(int cableCategoryId)
        {
            return _context.Cables.Include(c => c.CableCategory).FirstOrDefault(c => c.CableCategoryId == cableCategoryId && c.IsInRequest == false);
        }

        public List<CableCategory> getListCableCategory(int? warehouseId)
        {
            IQueryable<Cable> query = getQuery(null, warehouseId, false);
            return query.Select(c => c.CableCategory).Distinct().ToList();
        }

        public int getSum(int cableCategoryId, int? warehouseId)
        {
            IQueryable<Cable> query = getQuery(null, warehouseId, false).Where(c => c.CableCategoryId == cableCategoryId);
            return query.Sum(c => c.Length);
        }

        public Cable? getCable(Guid cableId, int startPoint, int endPoint)
        {
            return _context.Cables.FirstOrDefault(c => c.CableParentId == cableId && c.StartPoint <= startPoint && c.EndPoint >= endPoint && c.IsDeleted == false);
        }

    }
}
