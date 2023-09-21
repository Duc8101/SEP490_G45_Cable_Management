using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class TransactionCable
    {
        public Guid TransactionId { get; set; }
        public Guid CableId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Cable Cable { get; set; } = null!;
        public virtual TransactionHistory Transaction { get; set; } = null!;
    }
}
