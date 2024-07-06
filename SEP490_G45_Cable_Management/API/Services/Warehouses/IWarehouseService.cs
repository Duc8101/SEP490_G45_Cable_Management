using Common.Base;
using Common.DTO.WarehouseDTO;

namespace API.Services.Warehouses
{
    public interface IWarehouseService
    {
        ResponseBase ListPaged(string? name, int page);
        ResponseBase ListAll();
        ResponseBase Create(WarehouseCreateUpdateDTO DTO, Guid creatorId);
        ResponseBase Update(int warehouseId, WarehouseCreateUpdateDTO DTO);
        ResponseBase Delete(int warehouseId);
    }
}
