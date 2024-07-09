using Common.Base;
using Common.DTO.CableCategoryDTO;

namespace API.Services.CableCategories
{
    public interface ICableCategoryService
    {
        ResponseBase ListPaged(string? name, int page);
        ResponseBase ListAll();
        ResponseBase Create(CableCategoryCreateUpdateDTO DTO);
        ResponseBase Update(int cableCategoryId, CableCategoryCreateUpdateDTO DTO);
    }
}
