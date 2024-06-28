using Common.Base;
using Common.DTO.OtherMaterialsDTO;
using Common.Pagination;

namespace API.Services.OtherMaterials
{
    public interface IOtherMaterialsService
    {
        Task<ResponseBase<Pagination<OtherMaterialsListDTO>?>> ListPaged(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page);
        Task<ResponseBase<List<OtherMaterialsListDTO>?>> ListAll(int? WareHouseID);
        Task<ResponseBase<bool>> Create(OtherMaterialsCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(int OtherMaterialsID);
    }
}
