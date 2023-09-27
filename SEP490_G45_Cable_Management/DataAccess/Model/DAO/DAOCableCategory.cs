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
    }
}
