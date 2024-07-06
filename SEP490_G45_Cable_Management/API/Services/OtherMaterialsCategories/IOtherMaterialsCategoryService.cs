using Common.Base;
using Common.DTO.OtherMaterialsCategoryDTO;

namespace API.Services.OtherMaterialsCategories
{
    public interface IOtherMaterialsCategoryService
    {
        ResponseBase ListPaged(string? name, int page);
        ResponseBase ListAll();
        ResponseBase Create(OtherMaterialsCategoryCreateUpdateDTO DTO);
        ResponseBase Update(int otherMaterialsCategoryId, OtherMaterialsCategoryCreateUpdateDTO DTO);
    }
}
