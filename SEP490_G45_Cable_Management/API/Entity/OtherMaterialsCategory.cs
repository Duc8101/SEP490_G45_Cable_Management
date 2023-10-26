using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class OtherMaterialsCategory
    {
        public OtherMaterialsCategory()
        {
            NodeMaterialCategories = new HashSet<NodeMaterialCategory>();
            OtherMaterials = new HashSet<OtherMaterial>();
        }

        public int OtherMaterialsCategoryId { get; set; }
        public string? OtherMaterialsCategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<NodeMaterialCategory> NodeMaterialCategories { get; set; }
        public virtual ICollection<OtherMaterial> OtherMaterials { get; set; }
    }
}
