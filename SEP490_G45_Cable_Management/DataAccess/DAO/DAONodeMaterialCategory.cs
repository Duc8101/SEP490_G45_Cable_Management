using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAONodeMaterialCategory : BaseDAO
    {
        public DAONodeMaterialCategory(CableManagementContext context) : base(context)
        {
        }

        public NodeMaterialCategory? getNodeMaterialCategory(int otherMaterialCategoryId, Guid nodeId)
        {
            return _context.NodeMaterialCategories.FirstOrDefault(n => n.OtherMaterialCategoryId == otherMaterialCategoryId && n.IsDeleted == false && n.NodeId == nodeId);
        }

        public void CreateNodeMaterialCategory(NodeMaterialCategory material)
        {
            _context.NodeMaterialCategories.Add(material);
            Save();
        }

        public void UpdateNodeMaterialCategory(NodeMaterialCategory material)
        {
            _context.NodeMaterialCategories.Update(material);
            Save();
        }

        public List<NodeMaterialCategory> getListNodeMaterialCategory(Guid nodeId)
        {
            return _context.NodeMaterialCategories.Include(n => n.OtherMaterialCategory).OrderByDescending(n => n.UpdateAt)
                .Where(n => n.NodeId == nodeId && n.IsDeleted == false).ToList();
        }

        public List<OtherMaterialsCategory> getListOtherMaterialCategory(Guid routeId)
        {
            return _context.NodeMaterialCategories.Where(n => n.Node.RouteId == routeId && n.Node.IsDeleted == false && n.IsDeleted == false)
                .Select(n => n.OtherMaterialCategory).Distinct().ToList();
        }

        public int getSumQuantity(Guid routeId, int otherMaterialCategoryId)
        {
            return _context.NodeMaterialCategories.Where(n => n.Node.RouteId == routeId && n.Node.IsDeleted == false && n.IsDeleted == false && n.OtherMaterialCategoryId == otherMaterialCategoryId)
                .Sum(n => n.Quantity);
        }
    }
}
