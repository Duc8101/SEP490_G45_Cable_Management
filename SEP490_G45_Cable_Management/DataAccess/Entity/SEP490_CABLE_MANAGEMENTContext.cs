using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Entity
{
    public partial class SEP490_CABLE_MANAGEMENTContext : DbContext
    {
        public SEP490_CABLE_MANAGEMENTContext()
        {
        }

        public SEP490_CABLE_MANAGEMENTContext(DbContextOptions<SEP490_CABLE_MANAGEMENTContext> options)
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
                ConfigurationBuilder builder = new ConfigurationBuilder();
                IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
                string connect = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connect);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cable>(entity =>
            {
                entity.ToTable("Cable");

                entity.Property(e => e.CableId)
                    .ValueGeneratedNever()
                    .HasColumnName("CableID");

                entity.Property(e => e.CableCategoryId).HasColumnName("CableCategoryID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsExportedToUse).HasColumnName("isExportedToUse");

                entity.Property(e => e.IsInRequest).HasColumnName("isInRequest");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

                entity.HasOne(d => d.CableCategory)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.CableCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cable__CableCate__66603565");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cable__CreatorID__656C112C");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cable__SupplierI__6477ECF3");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Cables)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cable__Warehouse__6383C8BA");
            });

            modelBuilder.Entity<CableCategory>(entity =>
            {
                entity.ToTable("CableCategory");

                entity.Property(e => e.CableCategoryId).HasColumnName("CableCategoryID");

                entity.Property(e => e.CableCategoryName).HasMaxLength(100);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("Issue");

                entity.Property(e => e.IssueId)
                    .ValueGeneratedNever()
                    .HasColumnName("IssueID");

                entity.Property(e => e.CableRoutingName).HasMaxLength(100);

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Group).HasMaxLength(50);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IssueCode).HasMaxLength(50);

                entity.Property(e => e.IssueName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Issue__CreatorID__5EBF139D");
            });

            modelBuilder.Entity<Node>(entity =>
            {
                entity.ToTable("Node");

                entity.Property(e => e.NodeId)
                    .ValueGeneratedNever()
                    .HasColumnName("NodeID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Nodes)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Node__RouteID__5FB337D6");
            });

            modelBuilder.Entity<NodeCable>(entity =>
            {
                entity.ToTable("NodeCable");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CableId).HasColumnName("CableID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.NodeCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeCable__Cable__6754599E");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeCables)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeCable__NodeI__68487DD7");
            });

            modelBuilder.Entity<NodeMaterial>(entity =>
            {
                entity.ToTable("NodeMaterial");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.OtherMaterialsId).HasColumnName("OtherMaterialsID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeMaterials)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeMater__NodeI__6EF57B66");

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.NodeMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeMater__Other__6E01572D");
            });

            modelBuilder.Entity<NodeMaterialCategory>(entity =>
            {
                entity.ToTable("NodeMaterialCategory");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.NodeId).HasColumnName("NodeID");

                entity.Property(e => e.OtherMaterialsCategoryId).HasColumnName("OtherMaterialsCategoryID");

                entity.HasOne(d => d.Node)
                    .WithMany(p => p.NodeMaterialCategories)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeMater__NodeI__6A30C649");

                entity.HasOne(d => d.OtherMaterialsCategory)
                    .WithMany(p => p.NodeMaterialCategories)
                    .HasForeignKey(d => d.OtherMaterialsCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NodeMater__Other__693CA210");
            });

            modelBuilder.Entity<OtherMaterial>(entity =>
            {
                entity.HasKey(e => e.OtherMaterialsId)
                    .HasName("PK__OtherMat__14E82B14EC0B3293");

                entity.Property(e => e.OtherMaterialsId).HasColumnName("OtherMaterialsID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.OtherMaterialsCategoryId).HasColumnName("OtherMaterialsCategoryID");

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Unit).HasMaxLength(15);

                entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

                entity.HasOne(d => d.OtherMaterialsCategory)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OtherMate__Other__6D0D32F4");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OtherMate__Suppl__6B24EA82");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.OtherMaterials)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OtherMate__Wareh__6C190EBB");
            });

            modelBuilder.Entity<OtherMaterialsCategory>(entity =>
            {
                entity.ToTable("OtherMaterialsCategory");

                entity.Property(e => e.OtherMaterialsCategoryId).HasColumnName("OtherMaterialsCategoryID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.OtherMaterialsCategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request");

                entity.Property(e => e.RequestId)
                    .ValueGeneratedNever()
                    .HasColumnName("RequestID");

                entity.Property(e => e.ApproverId).HasColumnName("ApproverID");

                entity.Property(e => e.Content).HasMaxLength(255);

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.DeliverWarehouseId).HasColumnName("DeliverWarehouseID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IssueId).HasColumnName("IssueID");

                entity.Property(e => e.RequestCategoryId).HasColumnName("RequestCategoryID");

                entity.Property(e => e.RequestName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.RequestApprovers)
                    .HasForeignKey(d => d.ApproverId)
                    .HasConstraintName("FK__Request__Approve__70DDC3D8");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.RequestCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Request__Creator__6FE99F9F");

                entity.HasOne(d => d.DeliverWarehouse)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.DeliverWarehouseId)
                    .HasConstraintName("FK__Request__Deliver__73BA3083");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Request__IssueID__71D1E811");

                entity.HasOne(d => d.RequestCategory)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Request__Request__72C60C4A");
            });

            modelBuilder.Entity<RequestCable>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.CableId, e.StartPoint, e.EndPoint })
                    .HasName("PK__RequestC__8A3E7AD00A4D9B91");

                entity.ToTable("RequestCable");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.CableId).HasColumnName("CableID");

                entity.Property(e => e.ImportedWarehouseId).HasColumnName("ImportedWarehouseID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RecoveryDestWarehouseId).HasColumnName("RecoveryDestWarehouseID");

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.RequestCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestCa__Cable__75A278F5");

                entity.HasOne(d => d.ImportedWarehouse)
                    .WithMany(p => p.RequestCableImportedWarehouses)
                    .HasForeignKey(d => d.ImportedWarehouseId)
                    .HasConstraintName("FK__RequestCa__Impor__778AC167");

                entity.HasOne(d => d.RecoveryDestWarehouse)
                    .WithMany(p => p.RequestCableRecoveryDestWarehouses)
                    .HasForeignKey(d => d.RecoveryDestWarehouseId)
                    .HasConstraintName("FK__RequestCa__Recov__76969D2E");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestCables)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestCa__Reque__74AE54BC");
            });

            modelBuilder.Entity<RequestCategory>(entity =>
            {
                entity.Property(e => e.RequestCategoryId).HasColumnName("RequestCategoryID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RequestCategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<RequestOtherMaterial>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.OtherMaterialsId, e.Quantity })
                    .HasName("PK__RequestO__D03A972A29721FC3");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.OtherMaterialsId).HasColumnName("OtherMaterialsID");

                entity.Property(e => e.ImportedWarehouseId).HasColumnName("ImportedWarehouseID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RecoveryDestWarehouseId).HasColumnName("RecoveryDestWarehouseID");

                entity.Property(e => e.Status).HasMaxLength(15);

                entity.HasOne(d => d.ImportedWarehouse)
                    .WithMany(p => p.RequestOtherMaterialImportedWarehouses)
                    .HasForeignKey(d => d.ImportedWarehouseId)
                    .HasConstraintName("FK__RequestOt__Impor__7B5B524B");

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.RequestOtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestOt__Other__797309D9");

                entity.HasOne(d => d.RecoveryDestWarehouse)
                    .WithMany(p => p.RequestOtherMaterialRecoveryDestWarehouses)
                    .HasForeignKey(d => d.RecoveryDestWarehouseId)
                    .HasConstraintName("FK__RequestOt__Recov__7A672E12");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestOtherMaterials)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RequestOt__Reque__787EE5A0");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");

                entity.Property(e => e.RouteId)
                    .ValueGeneratedNever()
                    .HasColumnName("RouteID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.SupplierName).HasMaxLength(50);

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Supplier__Creato__60A75C0F");
            });

            modelBuilder.Entity<TransactionCable>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.CableId })
                    .HasName("PK__Transact__FC2F10D44F914028");

                entity.ToTable("TransactionCable");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.CableId).HasColumnName("CableID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.Cable)
                    .WithMany(p => p.TransactionCables)
                    .HasForeignKey(d => d.CableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Cable__02084FDA");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionCables)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Trans__01142BA1");
            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Transact__55433A4B42E070AF");

                entity.ToTable("TransactionHistory");

                entity.Property(e => e.TransactionId)
                    .ValueGeneratedNever()
                    .HasColumnName("TransactionID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.FromWarehouseId).HasColumnName("FromWarehouseID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IssueId).HasColumnName("IssueID");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.ToWareHouseId).HasColumnName("ToWareHouseID");

                entity.Property(e => e.TransactionCategoryName).HasMaxLength(50);

                entity.Property(e => e.WareHouseId).HasColumnName("WareHouseID");

                entity.HasOne(d => d.FromWarehouse)
                    .WithMany(p => p.TransactionHistoryFromWarehouses)
                    .HasForeignKey(d => d.FromWarehouseId)
                    .HasConstraintName("FK__Transacti__FromW__7D439ABD");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.IssueId)
                    .HasConstraintName("FK__Transacti__Issue__7F2BE32F");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Reque__00200768");

                entity.HasOne(d => d.ToWareHouse)
                    .WithMany(p => p.TransactionHistoryToWareHouses)
                    .HasForeignKey(d => d.ToWareHouseId)
                    .HasConstraintName("FK__Transacti__ToWar__7E37BEF6");

                entity.HasOne(d => d.WareHouse)
                    .WithMany(p => p.TransactionHistoryWareHouses)
                    .HasForeignKey(d => d.WareHouseId)
                    .HasConstraintName("FK__Transacti__WareH__7C4F7684");
            });

            modelBuilder.Entity<TransactionOtherMaterial>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.OtherMaterialsId })
                    .HasName("PK__Transact__040DB8FA4376F547");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.OtherMaterialsId).HasColumnName("OtherMaterialsID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.OtherMaterials)
                    .WithMany(p => p.TransactionOtherMaterials)
                    .HasForeignKey(d => d.OtherMaterialsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Other__03F0984C");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionOtherMaterials)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Trans__02FC7413");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__RoleID__5DCAEF64");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.WarehouseKeeperId).HasColumnName("WarehouseKeeperID");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.WarehouseCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Warehouse__Creat__619B8048");

                entity.HasOne(d => d.WarehouseKeeper)
                    .WithMany(p => p.WarehouseWarehouseKeepers)
                    .HasForeignKey(d => d.WarehouseKeeperId)
                    .HasConstraintName("FK__Warehouse__Wareh__628FA481");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
