using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class Route
    {
        public Route()
        {
            Nodes = new HashSet<Node>();
        }

        public Guid RouteId { get; set; }
        public string RouteName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Node> Nodes { get; set; }
    }
}
