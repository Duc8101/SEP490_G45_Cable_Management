using DataAccess.DTO.CableCategoryDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface ICableCategoryService
    {
        Task<ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseDTO<List<CableCategoryListDTO>?>> ListAll();
        ResponseDTO<bool> Create(CableCategoryCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO);
    }
}
