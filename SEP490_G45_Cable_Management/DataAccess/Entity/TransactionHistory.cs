using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class TransactionHistory
    {
        public TransactionHistory()
        {
            TransactionCables = new HashSet<TransactionCable>();
            TransactionOtherMaterials = new HashSet<TransactionOtherMaterial>();
        }

        public Guid TransactionId { get; set; }
        public string? TransactionCategoryName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int? WareHouseId { get; set; }
        public Guid RequestId { get; set; }
        public int? FromWarehouseId { get; set; }
        public Guid? IssueId { get; set; }
        public int? ToWareHouseId { get; set; }

        public virtual Warehouse? FromWarehouse { get; set; }
        public virtual Issue? Issue { get; set; }
        public virtual Request Request { get; set; } = null!;
        public virtual Warehouse? ToWareHouse { get; set; }
        public virtual Warehouse? WareHouse { get; set; }
        public virtual ICollection<TransactionCable> TransactionCables { get; set; }
        public virtual ICollection<TransactionOtherMaterial> TransactionOtherMaterials { get; set; }
    }
}
