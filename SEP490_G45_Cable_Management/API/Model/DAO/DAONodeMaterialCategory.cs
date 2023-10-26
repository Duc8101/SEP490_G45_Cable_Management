using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAONodeMaterialCategory : BaseDAO
    {
        public async Task<NodeMaterialCategory?> getMaterial(int MaterialCategoryID)
        {
            return await context.NodeMaterialCategories.Where(n => n.OtherMaterialCategoryId == MaterialCategoryID).FirstOrDefaultAsync();
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
            return await context.NodeMaterialCategories.Where(n => n.NodeId == NodeID).ToListAsync();
        }
    }
}
