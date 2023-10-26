using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IOtherMaterialsCategoryService
    {
        Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseDTO<List<OtherMaterialsCategoryListDTO>?>> ListAll();
        ResponseDTO<bool> Create(OtherMaterialsCategoryCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Update(int OtherMaterialsCategoryID, OtherMaterialsCategoryCreateUpdateDTO DTO);
    }
}
