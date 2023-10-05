﻿using DataAccess.Const;
using DataAccess.DTO.CableDTO;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOCable : BaseDAO
    {
        private IQueryable<Cable> getQuery(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            IQueryable<Cable> query = context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == isExportedToUse);
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(c => c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())));
            }

            if(isExportedToUse == false && WarehouseID != null)
            {
                query = query.Where(c => c.WarehouseId == WarehouseID);
            }
            return query;
        }
        public async Task<List<Cable>> getList(string? filter, int? WarehouseID, bool isExportedToUse , int page)
        {
            IQueryable<Cable> query = getQuery(filter, WarehouseID, isExportedToUse);
            return await query.Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE)
                .OrderByDescending(c => c.UpdateAt).ToListAsync();
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
            return await context.Cables.AnyAsync(c => c.IsDeleted == false && c.Code == DTO.Code.Trim() && c.Status == DTO.Status.Trim()
            && c.SupplierId == DTO.SupplierId && c.StartPoint == DTO.StartPoint && c.EndPoint == DTO.EndPoint && c.YearOfManufacture == DTO.YearOfManufacture);
        }

        public async Task<int> CreateCable(Cable cable)
        {
            await context.Cables.AddAsync(cable);
            return await context.SaveChangesAsync();
        }

        public async Task<Cable?> getCable(Guid CableID)
        {
            return await context.Cables.SingleOrDefaultAsync(c => c.IsDeleted == false && c.CableId == CableID);
        }

        public async Task<Cable?> getCableIncludeDeleted(Guid CableID)
        {
            return await context.Cables.SingleOrDefaultAsync(c => c.CableId == CableID);
        }

        public async Task<int> UpdateCable(Cable cable)
        {
            context.Cables.Update(cable);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteCable(Cable cable)
        {
            cable.IsDeleted = true;
            return await UpdateCable(cable);
        }
    }
}
