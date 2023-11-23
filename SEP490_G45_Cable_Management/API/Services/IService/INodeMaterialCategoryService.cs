using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface INodeMaterialCategoryService
    {
        Task<ResponseDTO<bool>> Update(Guid NodeID, NodeMaterialCategoryUpdateDTO DTO);
    }
}
