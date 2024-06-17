using System;
using System.Collections.Generic;

namespace Common.Entity
{
    public partial class CableCategory
    {
        public CableCategory()
        {
            Cables = new HashSet<Cable>();
        }

        public int CableCategoryId { get; set; }
        public string CableCategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Cable> Cables { get; set; }
    }
}
