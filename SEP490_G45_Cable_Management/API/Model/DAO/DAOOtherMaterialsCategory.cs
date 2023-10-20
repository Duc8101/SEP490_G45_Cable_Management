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
    public class DAOOtherMaterialsCategory : BaseDAO
    {
        private IQueryable<OtherMaterialsCategory> getQuery(string? name)
        {
            IQueryable<OtherMaterialsCategory> query = context.OtherMaterialsCategories;
            if(name != null && name.Trim().Length != 0)
            {
                query = query.Where(o => o.OtherMaterialsCategoryName.ToLower().Contains(name.ToLower().Trim())); 
            }
            return query;
        }
        public async Task<List<OtherMaterialsCategory>> getListPaged(string? name, int page)
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(name);
            return await query.OrderByDescending(o => o.UpdateAt).Skip(PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name)
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(name);
            return await query.CountAsync();
        }

        public async Task<List<OtherMaterialsCategory>> getListAll()
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(null);
            return await query.OrderByDescending(o => o.UpdateAt).ToListAsync();
        }

        public async Task<bool> isExist(string OtherMaterialsCategoryName)
        {
            return await context.OtherMaterialsCategories.AnyAsync(o => o.OtherMaterialsCategoryName == OtherMaterialsCategoryName.Trim());
        }

        public async Task CreateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            await context.OtherMaterialsCategories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task<OtherMaterialsCategory?> getOtherMaterialsCategory(int OtherMaterialsCategoryID)
        {
            return await context.OtherMaterialsCategories.SingleOrDefaultAsync(o => o.OtherMaterialsCategoryId == OtherMaterialsCategoryID);
        }

        public async Task<bool> isExist(int OtherMaterialsCategoryID, string OtherMaterialsCategoryName)
        {
            return await context.OtherMaterialsCategories.AnyAsync(o => o.OtherMaterialsCategoryName == OtherMaterialsCategoryName.Trim() && o.OtherMaterialsCategoryId != OtherMaterialsCategoryID);
        }

        public async Task UpdateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            context.OtherMaterialsCategories.Update(category);
            await context.SaveChangesAsync();
        }
    }
}
