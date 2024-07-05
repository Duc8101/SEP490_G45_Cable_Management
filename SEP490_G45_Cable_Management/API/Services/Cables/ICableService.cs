using Common.Base;
using Common.DTO.CableDTO;

namespace API.Services.Cables
{
    public interface ICableService
    {
        ResponseBase ListPaged(string? filter, int? warehouseId, bool isExportedToUse, int page);
        ResponseBase ListAll(int? warehouseId);
        ResponseBase Create(CableCreateUpdateDTO DTO, Guid creatorId);
        ResponseBase Update(Guid cableId, CableCreateUpdateDTO DTO);
        ResponseBase Delete(Guid cableId);
    }
}
