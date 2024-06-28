using Common.Base;
using Common.DTO.CableCategoryDTO;
using Common.Pagination;

namespace API.Services.CableCategories
{
    public interface ICableCategoryService
    {
        Task<ResponseBase<Pagination<CableCategoryListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseBase<List<CableCategoryListDTO>?>> ListAll();
        Task<ResponseBase<bool>> Create(CableCategoryCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO);
    }
}
