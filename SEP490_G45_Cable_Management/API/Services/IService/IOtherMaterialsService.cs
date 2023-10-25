using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IOtherMaterialsService
    {
        Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page);
        Task<ResponseDTO<bool>> Create(OtherMaterialsCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(int OtherMaterialsID);
    }
}
