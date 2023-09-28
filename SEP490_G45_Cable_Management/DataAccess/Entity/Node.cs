﻿using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class Node
    {
        public Node()
        {
            NodeCables = new HashSet<NodeCable>();
            NodeMaterialCategories = new HashSet<NodeMaterialCategory>();
            NodeMaterials = new HashSet<NodeMaterial>();
        }

        public Guid NodeId { get; set; }
        public string? Description { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? Address { get; set; }
        public string NodeCode { get; set; } = null!;
        public string NodeNumberSign { get; set; } = null!;
        public int NumberOrder { get; set; }
        public Guid RouteId { get; set; }
        public string? Status { get; set; }

        public virtual Route Route { get; set; } = null!;
        public virtual ICollection<NodeCable> NodeCables { get; set; }
        public virtual ICollection<NodeMaterialCategory> NodeMaterialCategories { get; set; }
        public virtual ICollection<NodeMaterial> NodeMaterials { get; set; }
    }
}
