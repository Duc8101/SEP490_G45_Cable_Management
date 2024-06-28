using Common.Base;
using Common.DTO.OtherMaterialsCategoryDTO;
using Common.Pagination;

namespace API.Services.OtherMaterialsCategories
{
    public interface IOtherMaterialsCategoryService
    {
        Task<ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseBase<List<OtherMaterialsCategoryListDTO>?>> ListAll();
        Task<ResponseBase<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Update(int OtherMaterialsCategoryID, OtherMaterialsCategoryCreateUpdateDTO DTO);
    }
}
