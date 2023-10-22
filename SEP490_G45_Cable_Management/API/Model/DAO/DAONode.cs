using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAONode : BaseDAO
    {
        public async Task<List<Node>> getListNotDeleted(Guid RouteID)
        {
            return await context.Nodes.Include(n => n.NodeCables).ThenInclude(n => n.Cable).ThenInclude(n => n.CableCategory)
                .Include(n => n.NodeMaterials).ThenInclude(n => n.OtherMaterials).ThenInclude(n => n.OtherMaterialsCategory)
                .Include(n => n.NodeMaterialCategories.Where(n => n.IsDeleted == false)).ThenInclude(n => n.OtherMaterialCategory)
                .Where(n => n.IsDeleted == false && n.RouteId == RouteID).OrderByDescending(n => n.UpdateAt).ToListAsync();
        }
    }
}
