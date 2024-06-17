using API.Services.IService;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.RouteDTO;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Service
{
    public class RouteService : BaseService, IRouteService
    {
        private readonly DAORoute daoRoute = new DAORoute();

        public RouteService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<List<RouteListDTO>?>> ListAll(string? name)
        {
            try
            {
                List<Common.Entity.Route> list = await daoRoute.getListAll(name);
                List<RouteListDTO> result = _mapper.Map<List<RouteListDTO>>(list);
                return new ResponseBase<List<RouteListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<RouteListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase<Pagination<RouteListDTO>?>> ListPaged(int page)
        {
            try
            {
                List<Common.Entity.Route> list = await daoRoute.getListPaged(page);
                List<RouteListDTO> DTOs = _mapper.Map<List<RouteListDTO>>(list);
                int RowCount = await daoRoute.getRowCount();
                Pagination<RouteListDTO> result = new Pagination<RouteListDTO>(page, RowCount, PageSizeConst.MAX_ROUTE_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<RouteListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<RouteListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase<bool>> Create(RouteCreateDTO DTO)
        {
            if (DTO.RouteName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên tuyến không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if exist
                if (await daoRoute.isExist(DTO.RouteName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Tên tuyến đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                Common.Entity.Route route = new Common.Entity.Route()
                {
                    RouteId = Guid.NewGuid(),
                    RouteName = DTO.RouteName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                await daoRoute.CreateRoute(route);
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase<bool>> Delete(Guid RouteID)
        {
            try
            {
                Common.Entity.Route? route = await daoRoute.getRoute(RouteID);
                // if not found
                if (route == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy tên tuyến", (int)HttpStatusCode.NotFound);
                }
                await daoRoute.DeleteRoute(route);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
