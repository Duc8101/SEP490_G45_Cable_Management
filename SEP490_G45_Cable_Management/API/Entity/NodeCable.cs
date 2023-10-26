using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class NodeCable
    {
        public Guid Id { get; set; }
        public Guid CableId { get; set; }
        public Guid NodeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int OrderIndex { get; set; }

        public virtual Cable Cable { get; set; } = null!;
        public virtual Node Node { get; set; } = null!;
    }
}
