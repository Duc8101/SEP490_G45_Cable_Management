using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class Cable
    {
        public Cable()
        {
            NodeCables = new HashSet<NodeCable>();
            RequestCables = new HashSet<RequestCable>();
            TransactionCables = new HashSet<TransactionCable>();
        }

        public Guid CableId { get; set; }
        public int? WarehouseId { get; set; }
        public int? SupplierId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public int? YearOfManufacture { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public Guid CreatorId { get; set; }
        public Guid? CableParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsExportedToUse { get; set; }
        public int CableCategoryId { get; set; }
        public bool? IsInRequest { get; set; }
        public string? Description { get; set; }

        public virtual CableCategory CableCategory { get; set; } = null!;
        public virtual User Creator { get; set; } = null!;
        public virtual Supplier? Supplier { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public virtual ICollection<NodeCable> NodeCables { get; set; }
        public virtual ICollection<RequestCable> RequestCables { get; set; }
        public virtual ICollection<TransactionCable> TransactionCables { get; set; }
    }
}
