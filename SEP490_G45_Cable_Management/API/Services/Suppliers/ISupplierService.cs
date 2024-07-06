using Common.Base;
using Common.DTO.SupplierDTO;

namespace API.Services.Suppliers
{
    public interface ISupplierService
    {
        ResponseBase ListPaged(string? name, int page);
        ResponseBase ListAll();
        ResponseBase Create(SupplierCreateUpdateDTO DTO, Guid creatorId);
        ResponseBase Update(int supplierId, SupplierCreateUpdateDTO DTO);
        ResponseBase Delete(int supplierId);
    }
}
