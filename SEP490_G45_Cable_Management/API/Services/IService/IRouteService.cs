using Common.Base;
using Common.DTO.RouteDTO;
using Common.Pagination;

namespace API.Services.IService
{
    public interface IRouteService
    {
        Task<ResponseBase<List<RouteListDTO>?>> ListAll(string? name);
        Task<ResponseBase<Pagination<RouteListDTO>?>> ListPaged(int page);
        Task<ResponseBase<bool>> Create(RouteCreateDTO DTO);
        Task<ResponseBase<bool>> Delete(Guid RouteID);
    }
}
