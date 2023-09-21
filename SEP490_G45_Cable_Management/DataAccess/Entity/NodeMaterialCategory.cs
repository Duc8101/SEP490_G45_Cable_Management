using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class NodeMaterialCategory
    {
        public Guid Id { get; set; }
        public int OtherMaterialsCategoryId { get; set; }
        public Guid NodeId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Node Node { get; set; } = null!;
        public virtual OtherMaterialsCategory OtherMaterialsCategory { get; set; } = null!;
    }
}
