using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Entity
{
    public partial class CableManagementContext : DbContext
    {
        public CableManagementContext()
        {
        }

        public CableManagementContext(DbContextOptions<CableManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cable> Cables { get; set; } = null!;
        public virtual DbSet<CableCategory> CableCategories { get; set; } = null!;
        public virtual DbSet<Issue> Issues { get; set; } = null!;
        public virtual DbSet<Node> Nodes { get; set; } = null!;
        public virtual DbSet<NodeCable> NodeCables { get; set; } = null!;
        public virtual DbSet<NodeMaterial> NodeMaterials { get; set; } = null!;
        public virtual DbSet<NodeMaterialCategory> NodeMaterialCategories { get; set; } = null!;
        public virtual DbSet<OtherMaterial> OtherMaterials { get; set; } = null!;
        public virtual DbSet<OtherMaterialsCategory> OtherMaterialsCategories { get; set; } = null!;
        public virtual DbSet<Request> Requests { get; set; } = null!;
        public virtual DbSet<RequestCable> RequestCables { get; set; } = null!;
        public virtual DbSet<RequestCategory> RequestCategories { get; set; } = null!;
        public virtual DbSet<RequestOtherMaterial> RequestOtherMaterials { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Route> Routes { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<TransactionCable> TransactionCables { get; set; } = null!;
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; } = null!;
        public virtual DbSet<TransactionOtherMaterial> TransactionOtherMaterials { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Warehouse> Warehouses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server = LAPTOP-FNDSBAFO\\SQLDEV; database=CableManagement;Integrated security=true; TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cable>(entity =>
            {
                entity.ToTable("Cable");

                entity.HasIndex(e => e.CableCategoryId, "IX_Cable_CableCategoryId");

                entity.HasIndex(e => e.CreatorId, "IX_Cable_CreatorId");

                entity.HasIndex(e => e.SupplierId, "IX_Cable_SupplierId");

                entity.HasIndex(e => e.WarehouseId, "IX_Cable_WarehouseId");

                entity.Property(e => e.CableId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.IsExportedToUse)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.IsInRequest)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.CableCategory)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.CableCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cable__CreatorId__403A8C7D");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__Cable__SupplierI__3E52440B");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.WarehouseId)
                    .HasConstraintName("FK__Cable__Warehouse__3D5E1FD2");
            });

            modelBuilder.Entity<CableCategory>(entity =>
            {
                entity.ToTable("CableCategory");

                entity.Property(e => e.CableCategoryName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("(N'')");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("Issue");

                entity.HasIndex(e => e.CreatorId, "IX_Issue_CreatorId");

                entity.HasIndex(e => e.IssueCode, "IX_Issue_IssueCode")
                    .IsUnique()
                    .HasFilter("([IssueCode] IS NOT NULL)");

                entity.Property(e => e.IssueId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CableRoutingName).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Group).HasMaxLength(50);

                entity.Property(e => e.IssueCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IssueName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Issue__CreatorId__4F7CD00D");
            });

            modelBuilder.Entity<Node>(entity =>
            {
                entity.ToTable("Node");

                entity.HasIndex(e => e.RouteId, "IX_Node_RouteId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Nodes)
                    .HasForeignKey(d => d.RouteId);
            });

            modelBuilder.Entity<NodeCable>(entity =>
            {
                entity.ToTable("NodeCable");

                entity.HasIndex(e => e.CableId, "IX_NodeCable_CableId");

                entity.HasIndex(e => e.NodeId, "IX_NodeCable_NodeId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.NodeCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeCables)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<NodeMaterial>(entity =>
            {
                entity.ToTable("NodeMaterial");

                entity.HasIndex(e => e.NodeId, "IX_NodeMaterial_NodeId");

                entity.HasIndex(e => e.OtherMaterialsId, "IX_NodeMaterial_OtherMaterialsId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeMaterials)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.NodeMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<NodeMaterialCategory>(entity =>
            {
                entity.ToTable("NodeMaterialCategory");

                entity.HasIndex(e => e.NodeId, "IX_NodeMaterialCategory_NodeId");

                entity.HasIndex(e => e.OtherMaterialCategoryId, "IX_NodeMaterialCategory_OtherMaterialCategoryId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeMaterialCategories)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OtherMaterialCategory)
                    .WithMany(p => p.NodeMaterialCategories)
                    .HasForeignKey(d => d.OtherMaterialCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OtherMaterial>(entity =>
            {
                entity.HasKey(e => e.OtherMaterialsId)
                    .HasName("PK__OtherMat__14E82BF4B0F17E3B");

                entity.HasIndex(e => e.OtherMaterialsCategoryId, "IX_OtherMaterials_OtherMaterialsCategoryId");

                entity.HasIndex(e => e.SupplierId, "IX_OtherMaterials_SupplierId");

                entity.HasIndex(e => e.WarehouseId, "IX_OtherMaterials_WarehouseId");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.Property(e => e.Unit).HasMaxLength(15);

                entity.HasOne(d => d.OtherMaterialsCategory)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsCategoryId);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__OtherMate__Suppl__33D4B598");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.WarehouseId)
                    .HasConstraintName("FK__OtherMate__Wareh__36B12233");
            });

            modelBuilder.Entity<OtherMaterialsCategory>(entity =>
            {
                entity.ToTable("OtherMaterialsCategory");

                entity.Property(e => e.OtherMaterialsCategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request");

                entity.HasIndex(e => e.ApproverId, "IX_Request_ApproverId");

                entity.HasIndex(e => e.CreatorId, "IX_Request_CreatorId");

                entity.HasIndex(e => e.DeliverWarehouseId, "IX_Request_DeliverWarehouseId");

                entity.HasIndex(e => e.IssueId, "IX_Request_IssueId");

                entity.HasIndex(e => e.RequestCategoryId, "IX_Request_RequestCategoryId");

                entity.Property(e => e.RequestId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Content).HasMaxLength(255);

                entity.Property(e => e.RequestName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.RequestApprovers)
                    .HasForeignKey(d => d.ApproverId)
                    .HasConstraintName("FK__Request__Approve__5441852A");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.RequestCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Request__Creator__534D60F1");

                entity.HasOne(d => d.DeliverWarehouse)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.DeliverWarehouseId);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.IssueId)
                    .HasConstraintName("FK__Request__IssueId__5535A963");

                entity.HasOne(d => d.RequestCategory)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RequestCable>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.CableId, e.StartPoint, e.EndPoint })
                    .HasName("PK__RequestC__9AC47BE7FD0DAC77");

                entity.ToTable("RequestCable");

                entity.HasIndex(e => e.CableId, "IX_RequestCable_CableId");

                entity.HasIndex(e => e.ImportedWarehouseId, "IX_RequestCable_ImportedWarehouseId");

                entity.HasIndex(e => e.RecoveryDestWarehouseId, "IX_RequestCable_RecoveryDestWarehouseId");

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.RequestCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestCa__Cable__59063A47");

                entity.HasOne(d => d.ImportedWarehouse)
                    .WithMany(p => p.RequestCableImportedWarehouses)
                    .HasForeignKey(d => d.ImportedWarehouseId);

                entity.HasOne(d => d.RecoveryDestWarehouse)
                    .WithMany(p => p.RequestCableRecoveryDestWarehouses)
                    .HasForeignKey(d => d.RecoveryDestWarehouseId);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestCables)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestCa__Reque__5812160E");
            });

            modelBuilder.Entity<RequestCategory>(entity =>
            {
                entity.Property(e => e.RequestCategoryName).HasMaxLength(50);
            });

            modelBuilder.Entity<RequestOtherMaterial>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.OtherMaterialsId, e.Quantity })
                    .HasName("PK__RequestO__62E6D3C5FBF475BA");

                entity.HasIndex(e => e.ImportedWarehouseId, "IX_RequestOtherMaterials_ImportedWarehouseId");

                entity.HasIndex(e => e.OtherMaterialsId, "IX_RequestOtherMaterials_OtherMaterialsId");

                entity.HasIndex(e => e.RecoveryDestWarehouseId, "IX_RequestOtherMaterials_RecoveryDestWarehouseId");

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.HasOne(d => d.ImportedWarehouse)
                    .WithMany(p => p.RequestOtherMaterialImportedWarehouses)
                    .HasForeignKey(d => d.ImportedWarehouseId);

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.RequestOtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestOt__Other__5CD6CB2B");

                entity.HasOne(d => d.RecoveryDestWarehouse)
                    .WithMany(p => p.RequestOtherMaterialRecoveryDestWarehouses)
                    .HasForeignKey(d => d.RecoveryDestWarehouseId);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestOtherMaterials)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestOt__Reque__5BE2A6F2");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Rolename)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");

                entity.Property(e => e.RouteId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.HasIndex(e => e.CreatorId, "IX_Supplier_CreatorId");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierName).HasMaxLength(50);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.CreatorId)
                    .HasConstraintName("FK__Supplier__Creato__300424B4");
            });

            modelBuilder.Entity<TransactionCable>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.CableId })
                    .HasName("PK__Transact__FC2F10F68675DA26");

                entity.ToTable("TransactionCable");

                entity.HasIndex(e => e.CableId, "IX_TransactionCable_CableId");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.TransactionCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Cable__47DBAE45");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionCables)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Trans__46E78A0C");
            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Transact__55433A6B23692FC5");

                entity.ToTable("TransactionHistory");

                entity.HasIndex(e => e.FromWarehouseId, "IX_TransactionHistory_FromWarehouseId");

                entity.HasIndex(e => e.IssueId, "IX_TransactionHistory_IssueId");

                entity.HasIndex(e => e.RequestId, "IX_TransactionHistory_RequestId");

                entity.HasIndex(e => e.ToWarehouseId, "IX_TransactionHistory_ToWarehouseId");

                entity.HasIndex(e => e.WarehouseId, "IX_TransactionHistory_WarehouseId");

                entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.TransactionCategoryName).HasMaxLength(50);

                entity.HasOne(d => d.FromWarehouse)
                    .WithMany(p => p.TransactionHistoryFromWarehouses)
                    .HasForeignKey(d => d.FromWarehouseId);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.IssueId);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.RequestId);

                entity.HasOne(d => d.ToWarehouse)
                    .WithMany(p => p.TransactionHistoryToWarehouses)
                    .HasForeignKey(d => d.ToWarehouseId);

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.TransactionHistoryWarehouses)
                    .HasForeignKey(d => d.WarehouseId);
            });

            modelBuilder.Entity<TransactionOtherMaterial>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.OtherMaterialsId })
                    .HasName("PK__Transact__040DB8D40D72C562");

                entity.HasIndex(e => e.OtherMaterialsId, "IX_TransactionOtherMaterials_OtherMaterialsId");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.TransactionOtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Other__4BAC3F29");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionOtherMaterials)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Trans__4AB81AF0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

                entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname).HasMaxLength(50);

                entity.Property(e => e.Lastname).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__RoleId__276EDEB3");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(e => e.CreatorId, "IX_Warehouses_CreatorId");

                entity.HasIndex(e => e.WarehouseKeeperid, "IX_Warehouses_WarehouseKeeperid");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.WarehouseCreators)
                    .HasForeignKey(d => d.CreatorId);

                entity.HasOne(d => d.WarehouseKeeper)
                    .WithMany(p => p.WarehouseWarehouseKeepers)
                    .HasForeignKey(d => d.WarehouseKeeperid);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
