using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class Request
    {
        public Request()
        {
            RequestCables = new HashSet<RequestCable>();
            RequestOtherMaterials = new HashSet<RequestOtherMaterial>();
            TransactionHistories = new HashSet<TransactionHistory>();
        }

        public Guid RequestId { get; set; }
        public string? RequestName { get; set; }
        public string? Content { get; set; }
        public Guid CreatorId { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? IssueId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int RequestCategoryId { get; set; }
        public int? DeliverWarehouseId { get; set; }

        public virtual User? Approver { get; set; }
        public virtual User Creator { get; set; } = null!;
        public virtual Warehouse? DeliverWarehouse { get; set; }
        public virtual Issue? Issue { get; set; }
        public virtual RequestCategory RequestCategory { get; set; } = null!;
        public virtual ICollection<RequestCable> RequestCables { get; set; }
        public virtual ICollection<RequestOtherMaterial> RequestOtherMaterials { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}
