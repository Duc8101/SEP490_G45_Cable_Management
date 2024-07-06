using Common.Base;
using Common.DTO.NodeMaterialCategoryDTO;

namespace API.Services.NodeMaterialCategories
{
    public interface INodeMaterialCategoryService
    {
        ResponseBase Update(Guid nodeId, NodeMaterialCategoryUpdateDTO DTO);
    }
}
