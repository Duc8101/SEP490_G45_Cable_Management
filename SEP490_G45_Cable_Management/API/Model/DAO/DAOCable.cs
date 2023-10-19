using DataAccess.Const;
using DataAccess.DTO.CableDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOCable : BaseDAO
    {
        private IQueryable<Cable> getQuery(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            IQueryable<Cable> query = context.Cables.Include(c => c.CableCategory).Include(c => c.Supplier).Include(c => c.Warehouse)
                .Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == isExportedToUse);
            // if search base on name
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(c => c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || c.Code.ToLower().Contains(filter.ToLower().Trim()) || c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()));
            }
            // if not export and choose warehouse
            if (isExportedToUse == false && WarehouseID != null)
            {
                query = query.Where(c => c.WarehouseId == WarehouseID);
            }
            return query;
        }
        public async Task<List<Cable>> getList(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            IQueryable<Cable> query = getQuery(filter, WarehouseID, isExportedToUse);
            return await query.OrderByDescending(c => c.UpdateAt).Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            IQueryable<Cable> query = getQuery(filter, WarehouseID, isExportedToUse);
            return await query.CountAsync();
        }

        public async Task<int> getSum(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            IQueryable<Cable> query = getQuery(filter, WarehouseID, isExportedToUse);
            return await query.SumAsync(c => c.Length);
        }

        public async Task<bool> isExist(CableCreateUpdateDTO DTO)
        {
            return await context.Cables.AnyAsync(c => c.IsDeleted == false && c.Code == DTO.Code.Trim() && c.Status == (DTO.Status == null || DTO.Status.Length == 0 ? null : DTO.Status.Trim())
            && c.SupplierId == DTO.SupplierId && c.StartPoint == DTO.StartPoint && c.EndPoint == DTO.EndPoint 
            && c.YearOfManufacture == DTO.YearOfManufacture);
        }

        public async Task CreateCable(Cable cable)
        {
            await context.Cables.AddAsync(cable);
            await context.SaveChangesAsync();
        }

        public async Task<Cable?> getCable(Guid CableID)
        {
            return await context.Cables.SingleOrDefaultAsync(c => c.IsDeleted == false && c.CableId == CableID);
        }

        public async Task UpdateCable(Cable cable)
        {
            context.Cables.Update(cable);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCable(Cable cable)
        {
            cable.IsDeleted = true;
            await UpdateCable(cable);
        }

        public async Task<List<Cable>> getList(int CableCategoryID)
        {
            return await context.Cables.Include(c => c.CableCategory).Where(c => c.CableCategoryId == CableCategoryID).ToListAsync();
        }

        private async Task<List<Cable>> getList(int CableCategoryID, int? WareHouseID)
        {
            IQueryable<Cable> query = context.Cables.Where(c => c.CableCategoryId == CableCategoryID
            && c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false);
            // if choose warehouse
            if (WareHouseID != null)
            {
                query = query.Where(c => c.WarehouseId == WareHouseID);
            }
            return await query.ToListAsync();
        }

        public async Task<List<CableCategory>> getListCategory(int? WarehouseID)
        {
            IQueryable<Cable> query = getQuery(null, WarehouseID, false);
            List<CableCategory> result = await query.Select(c => c.CableCategory).Distinct().ToListAsync();
            return result;
        }

        public async Task<int> getSum(int CategoryID, int? WarehouseID)
        {
            List<Cable> list = await getList(CategoryID, WarehouseID);
            int sum = 0;
            foreach (Cable cable in list)
            {
                sum = sum + cable.Length;
            }
            return sum;
        }

        public async Task<Cable?> getCable(Guid CableID, int StartPoint, int EndPoint)
        {
            return await context.Cables.Where(c => c.CableParentId == CableID && c.StartPoint <= StartPoint && c.EndPoint >= EndPoint).FirstOrDefaultAsync();
        }

    }
}
