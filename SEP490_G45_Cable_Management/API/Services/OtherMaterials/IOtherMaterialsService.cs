using Common.Base;
using Common.DTO.OtherMaterialsDTO;

namespace API.Services.OtherMaterials
{
    public interface IOtherMaterialsService
    {
        ResponseBase ListPaged(string? filter, int? wareHouseId, Guid? wareHouseKeeperId, int page);
        ResponseBase ListAll(int? wareHouseId);
        ResponseBase Create(OtherMaterialsCreateUpdateDTO DTO);
        ResponseBase Update(int otherMaterialsId, OtherMaterialsCreateUpdateDTO DTO);
        ResponseBase Delete(int otherMaterialsId);
    }
}
