using API.Model.DAO;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.Entity;
using System.Net;

namespace API.Services.Service
{
    public class NodeMaterialCategoryService : INodeMaterialCategoryService
    {
        private readonly DAONodeMaterialCategory daoMaterial = new DAONodeMaterialCategory();
        public async Task<ResponseDTO<bool>> Create(NodeMaterialCategoryCreateDTO DTO)
        {
            try
            {
                foreach(MaterialCategoryDTO item in DTO.MaterialCategoryDTOs)
                {
                    NodeMaterialCategory? material = await daoMaterial.getMaterial(item.OtherMaterialsCategoryId);
                    // if material category not exist
                    if (material == null)
                    {
                        material = new NodeMaterialCategory()
                        {
                            Id = Guid.NewGuid(),
                            OtherMaterialCategoryId = item.OtherMaterialsCategoryId,
                            NodeId = DTO.NodeId,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        daoMaterial.CreateNodeMaterialCategory(material);
                    }
                    else
                    {
                        material.IsDeleted = false;
                        material.Quantity = item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        daoMaterial.UpdateNodeMaterialCategory(material);
                    }
                }
                return new ResponseDTO<bool>(true, "Update thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
