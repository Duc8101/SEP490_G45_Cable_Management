using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface INodeMaterialCategoryService
    {
        Task<ResponseDTO<bool>> Create(NodeMaterialCategoryCreateDTO DTO);
    }
}
