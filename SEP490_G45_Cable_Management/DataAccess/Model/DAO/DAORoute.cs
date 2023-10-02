using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAORoute : BaseDAO
    {
        public async Task<List<Route>> getList(string? name)
        {
            if(name == null || name.Trim().Length == 0)
            {
                return await context.Routes.Where(r => r.IsDeleted == false).OrderByDescending(r => r.CreatedAt).ToListAsync();
            }
            return await context.Routes.Where(r => r.IsDeleted == false && r.RouteName.ToLower().Contains(name.ToLower().Trim()))
                .OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<bool> isExist(string name)
        {
            return await context.Routes.AnyAsync(r => r.RouteName == name.Trim());
        }

        public async Task<int> CreateRoute(Route route)
        {
            await context.Routes.AddAsync(route);
            return await context.SaveChangesAsync();
        }
        public async Task<Route?> getRoute(Guid RouteID)
        {
            return await context.Routes.SingleOrDefaultAsync(r => r.RouteId == RouteID && r.IsDeleted == false);
        }

        public async Task<int> DeleteRoute(Route route)
        {
            route.IsDeleted = true;
            context.Routes.Update(route);
            return await context.SaveChangesAsync();
        }
    }
}
