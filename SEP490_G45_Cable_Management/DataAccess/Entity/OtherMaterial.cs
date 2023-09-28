using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class OtherMaterial
    {
        public OtherMaterial()
        {
            NodeMaterials = new HashSet<NodeMaterial>();
            RequestOtherMaterials = new HashSet<RequestOtherMaterial>();
            TransactionOtherMaterials = new HashSet<TransactionOtherMaterial>();
        }

        public int OtherMaterialsId { get; set; }
        public string Unit { get; set; } = null!;
        public int Quantity { get; set; }
        public string Code { get; set; } = null!;
        public int SupplierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int WarehouseId { get; set; }
        public string Status { get; set; } = null!;
        public int OtherMaterialsCategoryId { get; set; }

        public virtual OtherMaterialsCategory OtherMaterialsCategory { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual ICollection<NodeMaterial> NodeMaterials { get; set; }
        public virtual ICollection<RequestOtherMaterial> RequestOtherMaterials { get; set; }
        public virtual ICollection<TransactionOtherMaterial> TransactionOtherMaterials { get; set; }
    }
}
