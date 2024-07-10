using Common.Const;
using Common.Entity;
using DataAccess.DBContext;

namespace DataAccess.DAO
{
    public class DAORoute : BaseDAO
    {
        public DAORoute(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<Route> getQuery(string? name)
        {
            IQueryable<Route> query = _context.Routes.Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(r => r.RouteName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }

        public List<Route> getListRoute(string? name)
        {
            IQueryable<Route> query = getQuery(name);
            return query.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public List<Route> getListRoute(int page)
        {
            IQueryable<Route> query = getQuery(null);
            return query.OrderByDescending(r => r.CreatedAt).Skip((int)PageSize.Size * (page - 1)).Take((int)PageSize.Size)
                .ToList();
        }

        public int getRowCount()
        {
            IQueryable<Route> query = getQuery(null);
            return query.Count();
        }

        public bool isExist(string name)
        {
            return _context.Routes.Any(r => r.RouteName == name.Trim() && r.IsDeleted == false);
        }

        public void CreateRoute(Route route)
        {
            _context.Routes.Add(route);
            Save();
        }

        public Route? getRoute(Guid routeId)
        {
            return _context.Routes.SingleOrDefault(r => r.RouteId == routeId && r.IsDeleted == false);
        }

        public void DeleteRoute(Route route)
        {
            route.IsDeleted = true;
            _context.Routes.Update(route);
            Save();
        }
    }
}
