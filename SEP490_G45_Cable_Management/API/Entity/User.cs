using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class User
    {
        public User()
        {
            Cables = new HashSet<Cable>();
            Issues = new HashSet<Issue>();
            RequestApprovers = new HashSet<Request>();
            RequestCreators = new HashSet<Request>();
            Suppliers = new HashSet<Supplier>();
            WarehouseCreators = new HashSet<Warehouse>();
            WarehouseWarehouseKeepers = new HashSet<Warehouse>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Cable> Cables { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<Request> RequestApprovers { get; set; }
        public virtual ICollection<Request> RequestCreators { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
        public virtual ICollection<Warehouse> WarehouseCreators { get; set; }
        public virtual ICollection<Warehouse> WarehouseWarehouseKeepers { get; set; }
    }
}
