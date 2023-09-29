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
    public class DAOOtherMaterialsCategory : BaseDAO
    {
        public async Task<List<OtherMaterialsCategory>> getList(int page)
        {
            return await context.OtherMaterialsCategories.Skip(PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<bool> isExist(string OtherMaterialsCategoryName)
        {
            return await context.OtherMaterialsCategories.AnyAsync(o => o.OtherMaterialsCategoryName == OtherMaterialsCategoryName.Trim());
        }

        public async Task<int> CreateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            await context.OtherMaterialsCategories.AddAsync(category);
            return await context.SaveChangesAsync();
        }

        public async Task<OtherMaterialsCategory?> getOtherMaterialsCategory(int OtherMaterialsCategoryID)
        {
            return await context.OtherMaterialsCategories.SingleOrDefaultAsync(o => o.OtherMaterialsCategoryId == OtherMaterialsCategoryID);
        }

        public async Task<bool> isExist(int OtherMaterialsCategoryID, string OtherMaterialsCategoryName)
        {
            return await context.OtherMaterialsCategories.AnyAsync(o => o.OtherMaterialsCategoryName == OtherMaterialsCategoryName.Trim() && o.OtherMaterialsCategoryId != OtherMaterialsCategoryID);
        }

        public async Task<int> UpdateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            context.OtherMaterialsCategories.Update(category);
            return await context.SaveChangesAsync();
        }
    }
}
