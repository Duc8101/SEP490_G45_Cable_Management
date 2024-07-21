using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;

namespace DataAccess.DAO
{
    public class DAOOtherMaterialsCategory : BaseDAO
    {
        public DAOOtherMaterialsCategory(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<OtherMaterialsCategory> getQuery(string? name)
        {
            IQueryable<OtherMaterialsCategory> query = _context.OtherMaterialsCategories;
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(o => o.OtherMaterialsCategoryName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }

        public List<OtherMaterialsCategory> getListOtherMaterialsCategory(string? name, int page)
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(name);
            return query.OrderByDescending(o => o.UpdateAt).Skip((int)PageSize.Size * (page - 1)).Take((int)PageSize.Size)
                .ToList();
        }

        public int getRowCount(string? name)
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(name);
            return query.Count();
        }

        public List<OtherMaterialsCategory> getListOtherMaterialsCategory()
        {
            IQueryable<OtherMaterialsCategory> query = getQuery(null);
            return query.OrderByDescending(o => o.UpdateAt).ToList();
        }

        public bool isExist(string otherMaterialsCategoryName)
        {
            return _context.OtherMaterialsCategories.Any(o => o.OtherMaterialsCategoryName == otherMaterialsCategoryName.Trim());
        }

        public void CreateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            _context.OtherMaterialsCategories.Add(category);
            Save();
        }

        public OtherMaterialsCategory? getOtherMaterialsCategory(int otherMaterialsCategoryId)
        {
            return _context.OtherMaterialsCategories.SingleOrDefault(o => o.OtherMaterialsCategoryId == otherMaterialsCategoryId);
        }

        public bool isExist(int otherMaterialsCategoryId, string otherMaterialsCategoryName)
        {
            return _context.OtherMaterialsCategories.Any(o => o.OtherMaterialsCategoryName == otherMaterialsCategoryName.Trim() && o.OtherMaterialsCategoryId != otherMaterialsCategoryId);
        }

        public void UpdateOtherMaterialsCategory(OtherMaterialsCategory category)
        {
            _context.OtherMaterialsCategories.Update(category);
            Save();
        }
    }
}
