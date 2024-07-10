using Common.Const;
using Common.DTO.OtherMaterialsDTO;
using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOOtherMaterial : BaseDAO
    {
        public DAOOtherMaterial(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<OtherMaterial> getQuery(string? filter, int? wareHouseId, Guid? wareHouseKeeperId)
        {
            IQueryable<OtherMaterial> query = _context.OtherMaterials.Include(o => o.OtherMaterialsCategory).Include(o => o.Supplier).Include(o => o.Warehouse)
                .Where(o => o.IsDeleted == false);
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(o => o.OtherMaterialsCategory.OtherMaterialsCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || o.Warehouse != null && o.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim()));
            }
            // if login as warehouse keeper
            if (wareHouseKeeperId.HasValue)
            {
                query = query.Where(o => o.Warehouse != null && o.Warehouse.WarehouseKeeperid == wareHouseKeeperId);
            }
            // if choose warehouse
            if (wareHouseId.HasValue)
            {
                query = query.Where(o => o.WarehouseId == wareHouseId);
            }
            return query;
        }
        
        public List<OtherMaterial> getListOtherMaterial(string? filter, int? wareHouseId, Guid? wareHouseKeeperId, int page)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, wareHouseId, wareHouseKeeperId);
            return query.OrderByDescending(o => o.UpdateAt).Skip((int) PageSize.Size * (page - 1)).ToList();
        }

        public List<OtherMaterial> getListOtherMaterial(int? wareHouseId)
        {
            IQueryable<OtherMaterial> query = getQuery(null, wareHouseId, null);
            return query.OrderByDescending(o => o.UpdateAt).ToList();
        }

        public int getRowCount(string? filter, int? wareHouseId, Guid? wareHouseKeeperId)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, wareHouseId, wareHouseKeeperId);
            return query.Count();
        }

        public int getSum(string? filter, int? wareHouseId, Guid? wareHouseKeeperId)
        {
            IQueryable<OtherMaterial> query = getQuery(filter, wareHouseId, wareHouseKeeperId);
            return query.Sum(m => m.Quantity);
        }

        public OtherMaterial? getOtherMaterial(OtherMaterialsCreateUpdateDTO DTO)
        {
            return _context.OtherMaterials.FirstOrDefault(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.SupplierId == 1
            && o.Status == DTO.Status.Trim() && o.WarehouseId == DTO.WarehouseId);
        }

        public void CreateOtherMaterial(OtherMaterial material)
        {
            _context.OtherMaterials.Add(material);
            Save();
        }

        public void UpdateOtherMaterial(OtherMaterial material)
        {
            _context.OtherMaterials.Update(material);
            Save();
        }

        public OtherMaterial? getOtherMaterial(int otherMaterialId)
        {
            return _context.OtherMaterials.SingleOrDefault(o => o.OtherMaterialsId == otherMaterialId && o.IsDeleted == false);
        }

        public List<OtherMaterial> getListOtherMaterial(int otherMaterialCategoryId)
        {
            return _context.OtherMaterials.Include(o => o.OtherMaterialsCategory).Where(o => o.OtherMaterialsCategoryId == otherMaterialCategoryId)
                .ToList();
        }

        public bool isExist(int otherMaterialsId, OtherMaterialsCreateUpdateDTO DTO)
        {
            return _context.OtherMaterials.Any(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.Status == DTO.Status.Trim()
            && o.WarehouseId == DTO.WarehouseId && o.OtherMaterialsId != otherMaterialsId);
        }

        public void DeleteOtherMaterial(OtherMaterial material)
        {
            material.IsDeleted = true;
            UpdateOtherMaterial(material);
        }

        public List<OtherMaterialsCategory> getListOtherMaterialCategory(int? warehouseId)
        {
            IQueryable<OtherMaterial> query = getQuery(null, warehouseId, null);
            return query.Select(o => o.OtherMaterialsCategory).Distinct().ToList();
        }

        public int getSum(int otherMaterialCategoryId, int? warehouseId)
        {
            IQueryable<OtherMaterial> query = getQuery(null, warehouseId, null).Where(o => o.OtherMaterialsCategoryId == otherMaterialCategoryId);
            return query.Sum(o => o.Quantity);
        }

        public OtherMaterial? getOtherMaterial(int? wareHouseId, string? code, string status, string unit)
        {
            return _context.OtherMaterials.FirstOrDefault(o => o.WarehouseId == wareHouseId && o.Code == code && o.Status == status && o.Unit == unit && o.IsDeleted == false);
        }

        public OtherMaterial? getOtherMaterial(OtherMaterialsCancelOutsideDTO DTO)
        {
            return _context.OtherMaterials.Include(x => x.OtherMaterialsCategory).FirstOrDefault(o => o.Unit == DTO.Unit && o.Status == DTO.Status
            && o.SupplierId == DTO.SupplierId && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId && o.IsDeleted);
        }

        public OtherMaterial? getOtherMaterial(OtherMaterialsRecoveryDTO DTO)
        {
            return _context.OtherMaterials.FirstOrDefault(o => o.IsDeleted == false && o.OtherMaterialsCategoryId == DTO.OtherMaterialsCategoryId
            && o.Unit == DTO.Unit.Trim() && DTO.Code != null && o.Code == DTO.Code.Trim() && o.SupplierId == DTO.SupplierId
            && o.Status == DTO.Status.Trim() && o.WarehouseId == DTO.WarehouseId);
        }

    }
}
