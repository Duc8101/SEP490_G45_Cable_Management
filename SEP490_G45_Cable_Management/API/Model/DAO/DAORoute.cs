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
    public class DAORoute : BaseDAO
    {
        private IQueryable<DataAccess.Entity.Route> getQuery(string? name)
        {
            IQueryable<DataAccess.Entity.Route> query = context.Routes.Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(r => r.RouteName.ToLower().Contains(name.ToLower().Trim()));
            }
            return query;
        }
        public async Task<List<DataAccess.Entity.Route>> getListAll(string? name)
        {
            IQueryable<DataAccess.Entity.Route> query = getQuery(name);
            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }
        public async Task<List<DataAccess.Entity.Route>> getListPaged(int page)
        {
            IQueryable<DataAccess.Entity.Route> query = getQuery(null);
            return await query.OrderByDescending(r => r.CreatedAt).Skip(PageSizeConst.MAX_ROUTE_LIST_IN_PAGE * (page - 1))
                .ToListAsync();
        }

        public async Task<int> getRowCount()
        {
            IQueryable<DataAccess.Entity.Route> query = getQuery(null);
            return await query.CountAsync();
        }

        public async Task<bool> isExist(string name)
        {
            return await context.Routes.AnyAsync(r => r.RouteName == name.Trim() && r.IsDeleted == false);
        }

        public async Task CreateRoute(DataAccess.Entity.Route route)
        {
            await context.Routes.AddAsync(route);
            await context.SaveChangesAsync();
        }
        public async Task<DataAccess.Entity.Route?> getRoute(Guid RouteID)
        {
            return await context.Routes.SingleOrDefaultAsync(r => r.RouteId == RouteID && r.IsDeleted == false);
        }

        public async Task DeleteRoute(DataAccess.Entity.Route route)
        {
            route.IsDeleted = true;
            context.Routes.Update(route);
            await context.SaveChangesAsync();
        }
    }
}
