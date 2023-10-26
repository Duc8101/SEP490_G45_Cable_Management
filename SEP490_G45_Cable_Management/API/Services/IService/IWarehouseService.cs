using DataAccess.DTO.WarehouseDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IWarehouseService
    {
        Task<ResponseDTO<PagedResultDTO<WarehouseListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseDTO<List<WarehouseListDTO>?>> ListAll();
        ResponseDTO<bool> Create(WarehouseCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Update(int WarehouseID, WarehouseCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(int WarehouseID);
    }
}
