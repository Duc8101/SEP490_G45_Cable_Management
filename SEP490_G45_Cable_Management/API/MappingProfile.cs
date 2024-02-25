using AutoMapper;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.DTO.CableDTO;
using DataAccess.DTO.IssueDTO;
using DataAccess.DTO.NodeDTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.DTO.RequestDTO;
using DataAccess.DTO.RouteDTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.DTO.TransactionDTO;
using DataAccess.DTO.UserDTO;
using DataAccess.DTO.WarehouseDTO;
using DataAccess.Entity;

namespace API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CableCategory, CableCategoryListDTO>();
            CreateMap<Cable, CableListDTO>()
                .ForMember(des => des.WarehouseName, src => src.MapFrom(c => c.Warehouse == null ? null : c.Warehouse.WarehouseName))
                .ForMember(des => des.SupplierName, src => src.MapFrom(c => c.Supplier.SupplierName))
                .ForMember(des => des.CableCategoryName, src => src.MapFrom(c => c.CableCategory.CableCategoryName));
            CreateMap<CableCreateUpdateDTO, Cable>()
                .ForMember(des => des.Code, src => src.MapFrom(c => c.Code.Trim()))
                .ForMember(des => des.Status, src => src.MapFrom(c => c.Status.Trim()));
            CreateMap<Issue, IssueListDTO>();
            CreateMap<IssueCreateDTO, Issue>()
                .ForMember(des => des.IssueName, src => src.MapFrom(i => i.IssueName.Trim()))
                .ForMember(des => des.IssueCode, src => src.MapFrom(i => i.IssueCode.Trim()))
                .ForMember(des => des.Description, src => src.MapFrom(i => i.Description == null || i.Description.Trim().Length == 0 ? null : i.Description.Trim()))
                .ForMember(des => des.CableRoutingName, src => src.MapFrom(i => i.CableRoutingName == null || i.CableRoutingName.Trim().Length == 0 ? null : i.CableRoutingName.Trim()))
                .ForMember(des => des.Group, src => src.MapFrom(i => i.Group == null || i.Group.Trim().Length == 0 ? null : i.Group.Trim()));
            CreateMap<RequestCable, RequestCableByIssueDTO>()
                .ForMember(des => des.CableCategoryName, src => src.MapFrom(r => r.Cable.CableCategory.CableCategoryName));
            CreateMap<RequestOtherMaterial, RequestOtherMaterialsByIssueDTO>()
                .ForMember(des => des.OtherMaterialsCategoryName, src => src.MapFrom(r => r.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName));
            CreateMap<NodeMaterialCategory, NodeMaterialCategoryListDTO>()
                .ForMember(des => des.OtherMaterialsCategoryName, src => src.MapFrom(n => n.OtherMaterialCategory.OtherMaterialsCategoryName));
            CreateMap<Node, NodeListDTO>();
            CreateMap<OtherMaterialsCategory, OtherMaterialsCategoryListDTO>();
            CreateMap<OtherMaterial, OtherMaterialsListDTO>()
                .ForMember(des => des.WarehouseName, src => src.MapFrom(o => o.Warehouse == null ? null : o.Warehouse.WarehouseName))
                .ForMember(des => des.OtherMaterialsCategoryName, src => src.MapFrom(o => o.OtherMaterialsCategory.OtherMaterialsCategoryName));
            CreateMap<OtherMaterialsCreateUpdateDTO, OtherMaterial>()
                .ForMember(des => des.Unit, src => src.MapFrom(o => o.Unit.Trim()))
                .ForMember(des => des.Status, src => src.MapFrom(o => o.Status.Trim()));
            CreateMap<DataAccess.Entity.Route, RouteListDTO>();
            CreateMap<Supplier, SupplierListDTO>();
            CreateMap<SupplierCreateUpdateDTO, Supplier>()
                .ForMember(des => des.SupplierName, src => src.MapFrom(s => s.SupplierName.Trim()))
                .ForMember(des => des.Country, src => src.MapFrom(s => s.Country == null || s.Country.Trim().Length == 0 ? null : s.Country.Trim()))
                .ForMember(des => des.SupplierDescription, src => src.MapFrom(s => s.SupplierDescription == null || s.SupplierDescription.Trim().Length == 0 ? null : s.SupplierDescription.Trim()));
            CreateMap<TransactionHistory, TransactionHistoryDTO>()
                .ForMember(des => des.IssueCode, src => src.MapFrom(t => t.Issue == null ? null : t.Issue.IssueCode))
                .ForMember(des => des.FromWarehouseName, src => src.MapFrom(t => t.FromWarehouse == null ? null : t.FromWarehouse.WarehouseName))
                .ForMember(des => des.ToWarehouseName, src => src.MapFrom(t => t.ToWarehouse == null ? null : t.ToWarehouse.WarehouseName));
            CreateMap<TransactionCable, TransactionCableDTO>()
                .ForMember(des => des.CableCategoryName, src => src.MapFrom(t => t.Cable.CableCategory.CableCategoryName));
            CreateMap<TransactionOtherMaterial, TransactionMaterialDTO>()
                .ForMember(des => des.OtherMaterialsCategoryName, src => src.MapFrom(t => t.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName))
                .ForMember(des => des.Code, src => src.MapFrom(t => t.OtherMaterials.Code));
            CreateMap<TransactionHistory, TransactionDetailDTO>()
                .ForMember(des => des.IssueCode, src => src.MapFrom(t => t.Issue == null ? null : t.Issue.IssueCode))
                .ForMember(des => des.FromWarehouseName, src => src.MapFrom(t => t.FromWarehouse == null ? null : t.FromWarehouse.WarehouseName))
                .ForMember(des => des.ToWarehouseName, src => src.MapFrom(t => t.ToWarehouse == null ? null : t.ToWarehouse.WarehouseName));
            CreateMap<User, UserListDTO>()
                .ForMember(des => des.RoleName, src => src.MapFrom(t => t.Role.Rolename));
            CreateMap<UserCreateDTO, User>()
                .ForMember(des => des.Firstname, src => src.MapFrom(t => t.FirstName.Trim()))
                .ForMember(des => des.Lastname, src => src.MapFrom(t => t.LastName.Trim()))
                .ForMember(des => des.Username, src => src.MapFrom(t => t.UserName));
            CreateMap<Warehouse, WarehouseListDTO>()
                .ForMember(des => des.WarehouseKeeperName, src => src.MapFrom(w => w.WarehouseKeeper == null ? null : w.WarehouseKeeper.Lastname + " " + w.WarehouseKeeper.Firstname));
            CreateMap<Request, RequestListDTO>()
                .ForMember(des => des.CreatorName, src => src.MapFrom(r => r.Creator.Lastname + " " + r.Creator.Firstname))
                .ForMember(des => des.ApproverName, src => src.MapFrom(r => r.Approver == null ? null : r.Approver.Lastname + " " + r.Approver.Firstname))
                .ForMember(des => des.RequestCategoryName, src => src.MapFrom(r => r.RequestCategory.RequestCategoryName));
            CreateMap<RequestCable, RequestCableListDTO>()
                .ForMember(des => des.CableCategoryName, src => src.MapFrom(r => r.Cable.CableCategory.CableCategoryName))
                .ForMember(des => des.RecoveryDestWarehouseName, src => src.MapFrom(r => r.RecoveryDestWarehouse == null ? null : r.RecoveryDestWarehouse.WarehouseName));
            CreateMap<RequestOtherMaterial, RequestOtherMaterialsListDTO>()
                .ForMember(des => des.OtherMaterialsCategoryName, src => src.MapFrom(r => r.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName))
                .ForMember(des => des.RecoveryDestWarehouseName, src => src.MapFrom(r => r.RecoveryDestWarehouse == null ? null : r.RecoveryDestWarehouse.WarehouseName));
            CreateMap<Request, RequestDetailDTO>()
                .ForMember(des => des.CreatorName, src => src.MapFrom(r => r.Creator.Lastname + " " + r.Creator.Firstname))
                .ForMember(des => des.ApproverName, src => src.MapFrom(r => r.Approver == null ? null : r.Approver.Lastname + " " + r.Approver.Firstname))
                .ForMember(des => des.RequestCategoryName, src => src.MapFrom(r => r.RequestCategory.RequestCategoryName))
                .ForMember(des => des.IssueName, src => src.MapFrom(r => r.Issue == null ? null : r.Issue.IssueName))
                .ForMember(des => des.CableRoutingName, src => src.MapFrom(r => r.Issue == null ? null : r.Issue.CableRoutingName))
                .ForMember(des => des.DeliverWarehouseName, src => src.MapFrom(r => r.DeliverWarehouse == null ? null : r.DeliverWarehouse.WarehouseName));
        }
    }
}
