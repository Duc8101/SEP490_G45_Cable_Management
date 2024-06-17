using System;
using System.Collections.Generic;

namespace Common.Entity
{
    public partial class RequestCable
    {
        public Guid RequestId { get; set; }
        public Guid CableId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int? RecoveryDestWarehouseId { get; set; }
        public string? Status { get; set; }
        public int? ImportedWarehouseId { get; set; }

        public virtual Cable Cable { get; set; } = null!;
        public virtual Warehouse? ImportedWarehouse { get; set; }
        public virtual Warehouse? RecoveryDestWarehouse { get; set; }
        public virtual Request Request { get; set; } = null!;
    }
}
