using Common.Entity;
using DataAccess.DBContext;
using System.Data;

namespace DataAccess.DAO
{
    public class DAONode : BaseDAO
    {
        public DAONode(CableManagementContext context) : base(context)
        {
        }

        public List<Node> getListNode(Guid routeId, bool isSortedByUpdateAt)
        {
            IQueryable<Node> query = _context.Nodes.Where(n => n.IsDeleted == false && n.RouteId == routeId);
            if (isSortedByUpdateAt)
            {
                query = query.OrderByDescending(n => n.UpdateAt);
            }
            else
            {
                query = query.OrderBy(n => n.NumberOrder);
            }
            return query.ToList();
        }

        public void UpdateNode(Node node)
        {
            _context.Nodes.Update(node);
            Save();
        }

        public void CreateNode(Node node)
        {
            _context.Nodes.Add(node);
            Save();
        }

        public Node? getNode(Guid nodeId)
        {
            return _context.Nodes.SingleOrDefault(n => n.Id == nodeId && n.IsDeleted == false);
        }

        public void DeleteNode(Node node)
        {
            node.IsDeleted = true;
            UpdateNode(node);
        }

    }
}
