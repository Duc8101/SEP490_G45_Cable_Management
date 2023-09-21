using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class NodeMaterial
    {
        public Guid Id { get; set; }
        public int OtherMaterialsId { get; set; }
        public Guid NodeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int Quantity { get; set; }

        public virtual Node Node { get; set; } = null!;
        public virtual OtherMaterial OtherMaterials { get; set; } = null!;
    }
}
