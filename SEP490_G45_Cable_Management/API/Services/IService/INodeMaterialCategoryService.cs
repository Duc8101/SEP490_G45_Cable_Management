using Common.Base;
using Common.DTO.NodeMaterialCategoryDTO;

namespace API.Services.IService
{
    public interface INodeMaterialCategoryService
    {
        Task<ResponseBase<bool>> Update(Guid NodeID, NodeMaterialCategoryUpdateDTO DTO);
    }
}
