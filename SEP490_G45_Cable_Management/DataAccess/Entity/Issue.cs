using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class Issue
    {
        public Issue()
        {
            Requests = new HashSet<Request>();
            TransactionHistories = new HashSet<TransactionHistory>();
        }

        public Guid IssueId { get; set; }
        public string IssueName { get; set; } = null!;
        public string IssueCode { get; set; } = null!;
        public string? Description { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; } = null!;
        public string? CableRoutingName { get; set; }
        public string? Group { get; set; }

        public virtual User Creator { get; set; } = null!;
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}
