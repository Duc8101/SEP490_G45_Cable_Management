using DataAccess.DTO.SupplierDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface ISupplierService
    {
        Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseDTO<List<SupplierListDTO>?>> ListAll();
        ResponseDTO<bool> Create(SupplierCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(int SupplierID);
    }
}
