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
    }
}
