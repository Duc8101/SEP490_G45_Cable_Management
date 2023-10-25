using DataAccess.DTO.RouteDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IRouteService
    {
        Task<ResponseDTO<List<RouteListDTO>?>> ListAll(string? name);
        Task<ResponseDTO<PagedResultDTO<RouteListDTO>?>> ListPaged(int page);
        Task<ResponseDTO<bool>> Create(RouteCreateDTO DTO);
        Task<ResponseDTO<bool>> Delete(Guid RouteID);
    }
}
