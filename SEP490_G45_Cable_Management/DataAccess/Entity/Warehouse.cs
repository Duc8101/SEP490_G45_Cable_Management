using System;
using System.Collections.Generic;

namespace DataAccess.Entity
{
    public partial class Warehouse
    {
        public Warehouse()
        {
            Cables = new HashSet<Cable>();
            OtherMaterials = new HashSet<OtherMaterial>();
            RequestCableImportedWarehouses = new HashSet<RequestCable>();
            RequestCableRecoveryDestWarehouses = new HashSet<RequestCable>();
            RequestOtherMaterialImportedWarehouses = new HashSet<RequestOtherMaterial>();
            RequestOtherMaterialRecoveryDestWarehouses = new HashSet<RequestOtherMaterial>();
            Requests = new HashSet<Request>();
            TransactionHistoryFromWarehouses = new HashSet<TransactionHistory>();
            TransactionHistoryToWareHouses = new HashSet<TransactionHistory>();
            TransactionHistoryWareHouses = new HashSet<TransactionHistory>();
        }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public Guid? WarehouseKeeperId { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? WarehouseAddress { get; set; }

        public virtual User Creator { get; set; } = null!;
        public virtual User? WarehouseKeeper { get; set; }
        public virtual ICollection<Cable> Cables { get; set; }
        public virtual ICollection<OtherMaterial> OtherMaterials { get; set; }
        public virtual ICollection<RequestCable> RequestCableImportedWarehouses { get; set; }
        public virtual ICollection<RequestCable> RequestCableRecoveryDestWarehouses { get; set; }
        public virtual ICollection<RequestOtherMaterial> RequestOtherMaterialImportedWarehouses { get; set; }
        public virtual ICollection<RequestOtherMaterial> RequestOtherMaterialRecoveryDestWarehouses { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistoryFromWarehouses { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistoryToWareHouses { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistoryWareHouses { get; set; }
    }
}
