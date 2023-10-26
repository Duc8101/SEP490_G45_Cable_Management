using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOCableCategory : BaseDAO
    {
        private IQueryable<CableCategory> getQuery(string? name)
        {
            IQueryable<CableCategory> query = context.CableCategories;
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(c => c.CableCategoryName.ToLower().Contains(name.Trim().ToLower()));
            }
            return query;
        }
        public async Task<List<CableCategory>> getListPaged(string? name, int page)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return await query.Skip(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return await query.CountAsync();
        }
        public async Task<List<CableCategory>> getListAll()
        {
            IQueryable<CableCategory> query = getQuery(null);
            return await query.ToListAsync();
        }
        public void CreateCableCategory(CableCategory cable)
        {
            context.CableCategories.Add(cable);
            context.SaveChanges();
        }
        public bool isExist(string name)
        {
            return context.CableCategories.Any(c => c.CableCategoryName == name.Trim());
        }
        public async Task<CableCategory?> getCableCategory(int ID)
        {
            return await context.CableCategories.SingleOrDefaultAsync(c => c.CableCategoryId == ID);
        }
        public bool isExist(int ID, string name)
        {
            return context.CableCategories.Any(c => c.CableCategoryName == name.Trim() && c.CableCategoryId != ID);
        }
        public void UpdateCableCategory(CableCategory cable)
        {
            context.CableCategories.Update(cable);
            context.SaveChanges();
        }
    }
}
