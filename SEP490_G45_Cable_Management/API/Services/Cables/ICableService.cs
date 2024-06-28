using Common.Base;
using Common.DTO.CableDTO;
using Common.Pagination;

namespace API.Services.Cables
{
    public interface ICableService
    {
        Task<ResponseBase<Pagination<CableListDTO>?>> ListPaged(string? filter, int? WarehouseID, bool isExportedToUse, int page);
        Task<ResponseBase<List<CableListDTO>?>> ListAll(int? WarehouseID);
        Task<ResponseBase<bool>> Create(CableCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(Guid CableID);
    }
}
