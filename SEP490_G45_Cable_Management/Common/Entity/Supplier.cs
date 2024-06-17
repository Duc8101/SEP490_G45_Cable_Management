using System;
using System.Collections.Generic;

namespace Common.Entity
{
    public partial class Supplier
    {
        public Supplier()
        {
            Cables = new HashSet<Cable>();
            OtherMaterials = new HashSet<OtherMaterial>();
        }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? Country { get; set; }
        //public Guid? CreatorId { get; set; }
        public Guid CreatorId { get; set; }
        public string? SupplierDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User? Creator { get; set; }
        public virtual ICollection<Cable> Cables { get; set; }
        public virtual ICollection<OtherMaterial> OtherMaterials { get; set; }
    }
}
