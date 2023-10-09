using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAORoute : BaseDAO
    {
        public async Task<List<DataAccess.Entity.Route>> getList(string? name)
        {
            IQueryable<DataAccess.Entity.Route> query = context.Routes.Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(r => r.RouteName != null && r.RouteName.ToLower().Contains(name.ToLower().Trim()));
            }
            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<bool> isExist(string name)
        {
            return await context.Routes.AnyAsync(r => r.RouteName == name.Trim());
        }

        public async Task<int> CreateRoute(DataAccess.Entity.Route route)
        {
            await context.Routes.AddAsync(route);
            return await context.SaveChangesAsync();
        }
        public async Task<DataAccess.Entity.Route?> getRoute(Guid RouteID)
        {
            return await context.Routes.SingleOrDefaultAsync(r => r.RouteId == RouteID && r.IsDeleted == false);
        }

        public async Task<int> DeleteRoute(DataAccess.Entity.Route route)
        {
            route.IsDeleted = true;
            context.Routes.Update(route);
            return await context.SaveChangesAsync();
        }
    }
}
