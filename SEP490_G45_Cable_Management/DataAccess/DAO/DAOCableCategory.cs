using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;

namespace DataAccess.DAO
{
    public class DAOCableCategory : BaseDAO
    {
        public DAOCableCategory(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<CableCategory> getQuery(string? name)
        {
            IQueryable<CableCategory> query = _context.CableCategories;
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(c => c.CableCategoryName.ToLower().Contains(name.Trim().ToLower()));
            }
            return query;
        }

        public List<CableCategory> getListCableCategoryPaged(string? name, int page)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return query.OrderByDescending(c => c.UpdateAt).Skip((int)PageSize.Size * (page - 1))
                .Take((int)PageSize.Size).ToList();
        }

        public int getRowCount(string? name)
        {
            IQueryable<CableCategory> query = getQuery(name);
            return query.Count();
        }

        public List<CableCategory> getListCableCategoryAll()
        {
            return _context.CableCategories.OrderByDescending(c => c.UpdateAt).ToList();
        }

        public bool isExist(string name)
        {
            return _context.CableCategories.Any(c => c.CableCategoryName == name.Trim());
        }

        public void CreateCableCategory(CableCategory obj)
        {
            _context.CableCategories.Add(obj);
            Save();
        }

        public CableCategory? getCableCategory(int id)
        {
            return _context.CableCategories.Find(id);
        }
        public bool isExist(int id, string name)
        {
            return _context.CableCategories.Any(c => c.CableCategoryName == name.Trim() && c.CableCategoryId != id);
        }

        public void UpdateCableCategory(CableCategory obj)
        {
            _context.CableCategories.Update(obj);
            Save();
        }

    }
}
