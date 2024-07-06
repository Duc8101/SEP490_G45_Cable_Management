using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.NodeMaterialCategoryDTO;
using Common.Entity;
using DataAccess.DAO;
using System.Net;

namespace API.Services.NodeMaterialCategories
{
    public class NodeMaterialCategoryService : BaseService, INodeMaterialCategoryService
    {
        private readonly DAONodeMaterialCategory _daoNodeMaterialCategory;
        private readonly DAONode _daoNode;
        public NodeMaterialCategoryService(IMapper mapper, DAONodeMaterialCategory daoNodeMaterialCategory, DAONode daoNode) : base(mapper)
        {
            _daoNodeMaterialCategory = daoNodeMaterialCategory;
            _daoNode = daoNode;
        }

        public ResponseBase Update(Guid nodeId, NodeMaterialCategoryUpdateDTO DTO)
        {
            try
            {
                Node? node = _daoNode.getNode(nodeId);
                if (node == null)
                {
                    return new ResponseBase(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                // list item need to be updated
                List<NodeMaterialCategory> listUpdate = new List<NodeMaterialCategory>();
                foreach (MaterialCategoryDTO item in DTO.MaterialCategoryDTOs)
                {
                    NodeMaterialCategory? material = _daoNodeMaterialCategory.getNodeMaterialCategory(item.OtherMaterialsCategoryId, NodeID);
                    // if material category not exist
                    if (material == null)
                    {
                        material = new NodeMaterialCategory()
                        {
                            Id = Guid.NewGuid(),
                            OtherMaterialCategoryId = item.OtherMaterialsCategoryId,
                            NodeId = nodeId,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        _daoNodeMaterialCategory.CreateNodeMaterialCategory(material);
                        listUpdate.Add(material);
                    }
                    else
                    {
                        material.IsDeleted = false;
                        material.Quantity = item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        _daoNodeMaterialCategory.UpdateNodeMaterialCategory(material);
                        listUpdate.Add(material);
                    }
                }
                List<NodeMaterialCategory> listAll = _daoNodeMaterialCategory.getListNodeMaterialCategory(nodeId);
                if (listAll.Count > 0)
                {
                    foreach (NodeMaterialCategory item in listAll)
                    {
                        // if list update not contain item
                        if (!listUpdate.Contains(item))
                        {
                            item.IsDeleted = true;
                            item.UpdateAt = DateTime.Now;
                            _daoNodeMaterialCategory.UpdateNodeMaterialCategory(item);
                        }
                    }
                }
                return new ResponseBase(true, "Update thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
