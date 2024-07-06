using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.RouteDTO;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Routes
{
    public class RouteService : BaseService, IRouteService
    {
        private readonly DAORoute _daoRoute;

        public RouteService(IMapper mapper, DAORoute daoRoute) : base(mapper)
        {
            _daoRoute = daoRoute;
        }

        public ResponseBase Create(RouteCreateDTO DTO)
        {
            if (DTO.RouteName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên tuyến không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if exist
                if (_daoRoute.isExist(DTO.RouteName.Trim()))
                {
                    return new ResponseBase(false, "Tên tuyến đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                Common.Entity.Route route = new Common.Entity.Route()
                {
                    RouteId = Guid.NewGuid(),
                    RouteName = DTO.RouteName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                _daoRoute.CreateRoute(route);
                return new ResponseBase(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(Guid routeId)
        {
            try
            {
                Common.Entity.Route? route = _daoRoute.getRoute(routeId);
                // if not found
                if (route == null)
                {
                    return new ResponseBase(false, "Không tìm thấy tên tuyến", (int)HttpStatusCode.NotFound);
                }
                _daoRoute.DeleteRoute(route);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListAll(string? name)
        {
            try
            {
                List<Common.Entity.Route> list = _daoRoute.getListRoute(name);
                List<RouteListDTO> data = _mapper.Map<List<RouteListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPaged(int page)
        {
            try
            {
                List<Common.Entity.Route> list = _daoRoute.getListRoute(page);
                List<RouteListDTO> DTOs = _mapper.Map<List<RouteListDTO>>(list);
                int rowCount = _daoRoute.getRowCount();
                Pagination<RouteListDTO> data = new Pagination<RouteListDTO>()
                {
                    RowCount = rowCount,
                    CurrentPage = page,
                    List = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
