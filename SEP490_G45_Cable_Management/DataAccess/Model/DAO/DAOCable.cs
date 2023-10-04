using DataAccess.Const;
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
        public async Task<List<Cable>> getList(string? filter, int? WarehouseID, bool isExportedToUse , int page)
        {
            if(filter == null || filter.Trim().Length == 0)
            {
                if(isExportedToUse)
                {
                    return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true).Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE)
                    .OrderByDescending(c => c.UpdateAt).ToListAsync();
                }

                // if cable of all warehouse
                if(WarehouseID == null)
                {
                    return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false)
                        .Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE)
                    .OrderByDescending(c => c.UpdateAt).ToListAsync();
                }
                return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                && c.WarehouseId == WarehouseID).Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE)
                .OrderByDescending(c => c.UpdateAt).ToListAsync();
            }

            if (isExportedToUse)
            {
                return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true
                && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
               .Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE).OrderByDescending(c => c.UpdateAt).ToListAsync();
            }

            if (WarehouseID == null)
            {
                return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim()) || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim()))
                || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())) )).Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE)
                .OrderByDescending(c => c.UpdateAt).ToListAsync();
            }

            return await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
            && c.WarehouseId == WarehouseID && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
            || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
                .Skip(PageSizeConst.MAX_CABLE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_LIST_IN_PAGE).OrderByDescending(c => c.UpdateAt).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            List<Cable> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                if (isExportedToUse)
                {
                    list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true).ToListAsync();
                }
                else
                {
                    //if cable of all warehouse
                    if (WarehouseID == null)
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false).ToListAsync();
                    }
                    else
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && c.WarehouseId == WarehouseID).ToListAsync();
                    }
                }     
            }
            else
            {
                if (isExportedToUse)
                {
                    list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true
                    && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                    || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
                   .ToListAsync();
                }
                else
                {
                    //if cable of all warehouse
                    if (WarehouseID == null)
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim()) || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) 
                        || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())))).ToListAsync();
                    }
                    else
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && c.WarehouseId == WarehouseID && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                        || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
                        .ToListAsync();
                    }    
                }
            }
            return list.Count;
        }

        public async Task<int> getSum(string? filter, int? WarehouseID, bool isExportedToUse)
        {
            List<Cable> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                if (isExportedToUse)
                {
                    list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true).ToListAsync();
                }
                else
                {
                    // if cable of all warehouse
                    if (WarehouseID == null)
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false).ToListAsync();
                    }
                    else
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && c.WarehouseId == WarehouseID).ToListAsync();
                    }
                }
            }
            else
            {
                if (isExportedToUse)
                {
                    list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == true
                    && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                    || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
                   .ToListAsync();
                }
                else
                {
                    // if cable of all warehouse
                    if (WarehouseID == null)
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim()) || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim()))
                        || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim())))).ToListAsync();
                    }
                    else
                    {
                        list = await context.Cables.Where(c => c.IsDeleted == false && c.IsInRequest == false && c.IsExportedToUse == false
                        && c.WarehouseId == WarehouseID && (c.CableCategory.CableCategoryName.ToLower().Contains(filter.ToLower().Trim())
                        || (c.Code != null && c.Code.ToLower().Contains(filter.ToLower().Trim())) || (c.Supplier != null && c.Supplier.SupplierName.ToLower().Contains(filter.ToLower().Trim()))))
                        .ToListAsync();
                    }
                }
            }
            int sum = 0;
            foreach (Cable cable in list)
            {
                sum = sum + cable.Length;
            }
            return sum;
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
