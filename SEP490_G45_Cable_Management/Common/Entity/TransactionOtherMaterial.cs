using System;
using System.Collections.Generic;

namespace Common.Entity
{
    public partial class TransactionOtherMaterial
    {
        public Guid TransactionId { get; set; }
        public int OtherMaterialsId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual OtherMaterial OtherMaterials { get; set; } = null!;
        public virtual TransactionHistory Transaction { get; set; } = null!;
    }
}
