using DataAccess.DTO.CableDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface ICableService
    {
        Task<ResponseDTO<PagedResultDTO<CableListDTO>?>> ListPaged(string? filter, int? WarehouseID, bool isExportedToUse, int page);
        Task<ResponseDTO<List<CableListDTO>?>> ListAll(int? WarehouseID);
        ResponseDTO<bool> Create(CableCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(Guid CableID);
    }
}
