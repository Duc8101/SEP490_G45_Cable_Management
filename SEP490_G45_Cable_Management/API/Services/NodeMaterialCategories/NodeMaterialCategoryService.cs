using Common.Base;
using Common.DTO.NodeMaterialCategoryDTO;
using Common.Entity;
using DataAccess.DAO;
using System.Net;

namespace API.Services.NodeMaterialCategories
{
    public class NodeMaterialCategoryService : INodeMaterialCategoryService
    {
        private readonly DAONodeMaterialCategory daoMaterial = new DAONodeMaterialCategory();
        private readonly DAONode daoNode = new DAONode();
        public async Task<ResponseBase<bool>> Update(Guid NodeID, NodeMaterialCategoryUpdateDTO DTO)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                if (node == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                List<NodeMaterialCategory> listUpdate = new List<NodeMaterialCategory>();
                foreach (MaterialCategoryDTO item in DTO.MaterialCategoryDTOs)
                {
                    NodeMaterialCategory? material = await daoMaterial.getMaterial(item.OtherMaterialsCategoryId, NodeID);
                    // if material category not exist
                    if (material == null)
                    {
                        material = new NodeMaterialCategory()
                        {
                            Id = Guid.NewGuid(),
                            OtherMaterialCategoryId = item.OtherMaterialsCategoryId,
                            NodeId = NodeID,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        await daoMaterial.CreateNodeMaterialCategory(material);
                        listUpdate.Add(material);
                    }
                    else
                    {
                        material.IsDeleted = false;
                        material.Quantity = item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoMaterial.UpdateNodeMaterialCategory(material);
                        listUpdate.Add(material);
                    }
                }
                List<NodeMaterialCategory> listAll = await daoMaterial.getList(NodeID);
                if (listAll.Count > 0)
                {
                    foreach (NodeMaterialCategory item in listAll)
                    {
                        // if list update not contain item
                        if (!listUpdate.Contains(item))
                        {
                            item.IsDeleted = true;
                            item.UpdateAt = DateTime.Now;
                            await daoMaterial.UpdateNodeMaterialCategory(item);
                        }
                    }
                }
                return new ResponseBase<bool>(true, "Update thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
