using Common.Base;
using Common.DTO.WarehouseDTO;
using Common.Pagination;

namespace API.Services.Warehouses
{
    public interface IWarehouseService
    {
        Task<ResponseBase<Pagination<WarehouseListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseBase<List<WarehouseListDTO>?>> ListAll();
        Task<ResponseBase<bool>> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(int WarehouseID);
    }
}
