using Common.Base;
using Common.DTO.SupplierDTO;
using Common.Pagination;

namespace API.Services.IService
{
    public interface ISupplierService
    {
        Task<ResponseBase<Pagination<SupplierListDTO>?>> ListPaged(string? name, int page);
        Task<ResponseBase<List<SupplierListDTO>?>> ListAll();
        Task<ResponseBase<bool>> Create(SupplierCreateUpdateDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(int SupplierID);
    }
}
