﻿using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Model.DAO
{
    public class DAONode : BaseDAO
    {
        public async Task<List<Node>> getListNotDeleted(Guid RouteID)
        {
            return await context.Nodes.Where(n => n.IsDeleted == false && n.RouteId == RouteID).OrderByDescending(n => n.UpdateAt).ToListAsync();
        }
        public async Task<List<Node>> getListDeleted(Guid RouteID)
        {
            return await context.Nodes.Where(n => n.IsDeleted == true && n.RouteId == RouteID).ToListAsync();
        }
        public void UpdateNode(Node node)
        {
            context.Nodes.Update(node);
            context.SaveChanges();
        }
        public async Task<List<Node>> getListNodeOrderByNumberOrder(Guid RouteID)
        {
            return await context.Nodes.Where(n => n.IsDeleted == false && n.RouteId == RouteID).OrderBy(n => n.NumberOrder)
                .ToListAsync();
        }
        public void CreateNode(Node node)
        {
            context.Nodes.Add(node);
            context.SaveChanges();
        }
        public async Task<Node?> getNode(Guid NodeID)
        {
            return await context.Nodes.SingleOrDefaultAsync(n => n.Id == NodeID && n.IsDeleted == false);
        }
        public void DeleteNode(Node node)
        {
            node.IsDeleted = true;
            UpdateNode(node);
        }

    }
}
