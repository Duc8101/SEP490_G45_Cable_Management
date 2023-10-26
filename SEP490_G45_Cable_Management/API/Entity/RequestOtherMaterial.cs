using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class RequestOtherMaterial
    {
        public Guid RequestId { get; set; }
        public int OtherMaterialsId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int? RecoveryDestWarehouseId { get; set; }
        public string? Status { get; set; }
        public int? ImportedWarehouseId { get; set; }

        public virtual Warehouse? ImportedWarehouse { get; set; }
        public virtual OtherMaterial OtherMaterials { get; set; } = null!;
        public virtual Warehouse? RecoveryDestWarehouse { get; set; }
        public virtual Request Request { get; set; } = null!;
    }
}
