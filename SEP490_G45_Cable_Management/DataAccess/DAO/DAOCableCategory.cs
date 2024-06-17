using Common.Const;
using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOCableCategory : BaseDAO
    {
        private IQueryable<CableCategory> getQuery(string? name)
        {
            IQueryable<CableCategory> query = context.CableCategories;
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(c => c.CableCategoryName.ToLower().Contains(name.Trim().ToLower()));
            }
            return query;
        }
        public async Task<List<CableCategory>> getListPaged(string? name, int page)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return await query.OrderByDescending(c => c.UpdateAt).Skip(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return await query.CountAsync();
        }
        public async Task<List<CableCategory>> getListAll()
        {
            IQueryable<CableCategory> query = getQuery(null);
            return await query.OrderByDescending(c => c.UpdateAt).ToListAsync();
        }
        public async Task CreateCableCategory(CableCategory cable)
        {
            await context.CableCategories.AddAsync(cable);
            await context.SaveChangesAsync();
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
        public async Task UpdateCableCategory(CableCategory cable)
        {
            context.CableCategories.Update(cable);
            await context.SaveChangesAsync();
        }
    }
}
