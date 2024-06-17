using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAONodeMaterialCategory : BaseDAO
    {
        public async Task<NodeMaterialCategory?> getMaterial(int MaterialCategoryID, Guid NodeID)
        {
            return await context.NodeMaterialCategories.Where(n => n.OtherMaterialCategoryId == MaterialCategoryID && n.IsDeleted == false && n.NodeId == NodeID).FirstOrDefaultAsync();
        }
        public async Task CreateNodeMaterialCategory(NodeMaterialCategory material)
        {
            await context.NodeMaterialCategories.AddAsync(material);
            await context.SaveChangesAsync();
        }
        public async Task UpdateNodeMaterialCategory(NodeMaterialCategory material)
        {
            context.NodeMaterialCategories.Update(material);
            await context.SaveChangesAsync();
        }
        public async Task<List<NodeMaterialCategory>> getList(Guid NodeID)
        {
            return await context.NodeMaterialCategories.Include(n => n.OtherMaterialCategory).OrderByDescending(n => n.UpdateAt)
                .Where(n => n.NodeId == NodeID && n.IsDeleted == false).ToListAsync();
        }
        public async Task<List<OtherMaterialsCategory>> getListOtherMaterialCategory(Guid RouteID)
        {
            return await context.NodeMaterialCategories.Where(n => n.Node.RouteId == RouteID && n.Node.IsDeleted == false && n.IsDeleted == false).Select(n => n.OtherMaterialCategory).Distinct().ToListAsync();
        }
        public async Task<int> getSumQuantity(Guid RouteID, int OtherMaterialCategoryID)
        {
            return await context.NodeMaterialCategories.Where(n => n.Node.RouteId == RouteID && n.Node.IsDeleted == false && n.IsDeleted == false && n.OtherMaterialCategoryId == OtherMaterialCategoryID)
                .SumAsync(n => n.Quantity);
        }
    }
}
