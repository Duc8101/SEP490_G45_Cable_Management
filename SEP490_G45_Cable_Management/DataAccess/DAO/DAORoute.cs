using Common.Const;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAORoute : BaseDAO
    {
        private IQueryable<Common.Entity.Route> getQuery(string? name)
        {
            IQueryable<Common.Entity.Route> query = context.Routes.Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(r => r.RouteName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }
        public async Task<List<Common.Entity.Route>> getListAll(string? name)
        {
            IQueryable<Common.Entity.Route> query = getQuery(name);
            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
        public async Task<List<Common.Entity.Route>> getListPaged(int page)
        {
            IQueryable<Common.Entity.Route> query = getQuery(null);
            return await query.OrderByDescending(r => r.CreatedAt).Skip(PageSizeConst.MAX_ROUTE_LIST_IN_PAGE * (page - 1))
                .ToListAsync();
        }
        public async Task<int> getRowCount()
        {
            IQueryable<Common.Entity.Route> query = getQuery(null);
            return await query.CountAsync();
        }
        public async Task<bool> isExist(string name)
        {
            return await context.Routes.AnyAsync(r => r.RouteName == name.Trim() && r.IsDeleted == false);
        }
        public async Task CreateRoute(Common.Entity.Route route)
        {
            await context.Routes.AddAsync(route);
            await context.SaveChangesAsync();
        }
        public async Task<Common.Entity.Route?> getRoute(Guid RouteID)
        {
            return await context.Routes.SingleOrDefaultAsync(r => r.RouteId == RouteID && r.IsDeleted == false);
        }
        public async Task DeleteRoute(Common.Entity.Route route)
        {
            route.IsDeleted = true;
            context.Routes.Update(route);
            await context.SaveChangesAsync();
        }
    }
}
