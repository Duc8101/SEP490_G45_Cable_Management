using Common.Base;
using Common.DTO.RouteDTO;

namespace API.Services.Routes
{
    public interface IRouteService
    {
        ResponseBase ListAll(string? name);
        ResponseBase ListPaged(int page);
        ResponseBase Create(RouteCreateDTO DTO);
        ResponseBase Delete(Guid routeId);
    }
}
