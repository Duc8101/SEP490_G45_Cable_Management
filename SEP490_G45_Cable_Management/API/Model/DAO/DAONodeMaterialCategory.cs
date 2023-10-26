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
        public void CreateNodeMaterialCategory(NodeMaterialCategory material)
        {
            context.NodeMaterialCategories.Add(material);
            context.SaveChanges();
        }
        public void UpdateNodeMaterialCategory(NodeMaterialCategory material)
        {
            context.NodeMaterialCategories.Update(material);
            context.SaveChanges();
        }
        public async Task<List<NodeMaterialCategory>> getList(Guid NodeID)
        {
            return await context.NodeMaterialCategories.Where(n => n.NodeId == NodeID).ToListAsync();
        }
    }
}
