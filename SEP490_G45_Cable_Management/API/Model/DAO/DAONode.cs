using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Model.DAO
{
    public class DAONode : BaseDAO
    {
        public async Task<List<Node>> getList(Guid RouteID)
        {
            return await context.Nodes.Include(n => n.NodeCables).ThenInclude(n => n.Cable).ThenInclude(n => n.CableCategory)
                .Include(n => n.NodeMaterials).ThenInclude(n => n.OtherMaterials).ThenInclude(n => n.OtherMaterialsCategory)
                .Include(n => n.NodeMaterialCategories.Where(n => n.IsDeleted == false)).ThenInclude(n => n.OtherMaterialCategory)
                .Where(n => n.IsDeleted == false && n.RouteId == RouteID).OrderByDescending(n => n.UpdateAt).ToListAsync();
        }

        public async Task UpdateNode(Node node)
        {
            context.Nodes.Update(node);
            await context.SaveChangesAsync();
        }

        public async Task<List<Node>> getListNodeOrderByNumberOrder(Guid RouteID)
        {
            return await context.Nodes.Where(n => n.IsDeleted == false && n.RouteId == RouteID).OrderBy(n => n.NumberOrder)
                .ToListAsync();
        }

        public async Task CreateNode(Node node)
        {
            await context.Nodes.AddAsync(node);
            await context.SaveChangesAsync();
        }

    }
}
