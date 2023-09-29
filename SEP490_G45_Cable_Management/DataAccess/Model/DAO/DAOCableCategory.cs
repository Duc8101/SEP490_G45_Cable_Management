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
    public class DAOCableCategory : BaseDAO
    {
        public async Task<List<CableCategory>> getList(string? name, int page)
        {
            if(name == null || name.Trim().Length == 0)
            {
                return await context.CableCategories.Skip(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE).ToListAsync();
            }
            return await context.CableCategories.Where(c => c.CableCategoryName.ToLower().Contains(name.Trim().ToLower())).Skip(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? name)
        {
            List<CableCategory> list;
            if (name == null || name.Trim().Length == 0)
            {
                list = await context.CableCategories.ToListAsync();
            }
            else
            {
                list = await context.CableCategories.Where(c => c.CableCategoryName.ToLower().Contains(name.Trim().ToLower())).ToListAsync();
            }
            return list.Count;
        }

        public async Task<int> CreateCableCategory(CableCategory cable)
        {
            await context.CableCategories.AddAsync(cable);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> isExist(string name)
        {
            return await context.CableCategories.AnyAsync(c => c.CableCategoryName == name.Trim());
        }

        public async Task<CableCategory?> getCableCategory(int ID)
        {
            return await context.CableCategories.SingleOrDefaultAsync(c => c.CableCategoryId == ID);
        }

        public async Task<bool> isExist(int ID, string name)
        {
            return await context.CableCategories.AnyAsync(c => c.CableCategoryName == name.Trim() && c.CableCategoryId != ID);
        }

        public async Task<int> UpdateCableCategory(CableCategory cable)
        {
            context.CableCategories.Update(cable);
            return await context.SaveChangesAsync();
        }
    }
}
