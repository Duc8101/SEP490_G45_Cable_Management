﻿using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using System.Net;
using API.Model.DAO;
using System.Xml.Linq;
using DataAccess.Const;
using API.Services.IService;

namespace API.Services.Service
{
    public class RouteService : IRouteService
    {
        private readonly DAORoute daoRoute = new DAORoute();
        public async Task<ResponseDTO<List<RouteListDTO>?>> ListAll(string? name)
        {
            try
            {
                List<DataAccess.Entity.Route> list = await daoRoute.getListAll(name);
                List<RouteListDTO> result = new List<RouteListDTO>();
                foreach (DataAccess.Entity.Route route in list)
                {
                    RouteListDTO DTO = new RouteListDTO()
                    {
                        RouteId = route.RouteId,
                        RouteName = route.RouteName,
                    };
                    result.Add(DTO);
                }
                return new ResponseDTO<List<RouteListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<RouteListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<PagedResultDTO<RouteListDTO>?>> ListPaged(int page)
        {
            try
            {
                List<DataAccess.Entity.Route> list = await daoRoute.getListPaged(page);
                List<RouteListDTO> DTOs = new List<RouteListDTO>();
                foreach (DataAccess.Entity.Route route in list)
                {
                    RouteListDTO DTO = new RouteListDTO()
                    {
                        RouteId = route.RouteId,
                        RouteName = route.RouteName,
                    };
                    DTOs.Add(DTO);
                }
                int RowCount = await daoRoute.getRowCount();
                PagedResultDTO<RouteListDTO> result = new PagedResultDTO<RouteListDTO>(page, RowCount, PageSizeConst.MAX_ROUTE_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<RouteListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<RouteListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Create(RouteCreateDTO DTO)
        {
            if (DTO.RouteName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên tuyến không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if exist
                if (await daoRoute.isExist(DTO.RouteName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Tên tuyến đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                DataAccess.Entity.Route route = new DataAccess.Entity.Route()
                {
                    RouteId = Guid.NewGuid(),
                    RouteName = DTO.RouteName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                await daoRoute.CreateRoute(route);
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Delete(Guid RouteID)
        {
            try
            {
                DataAccess.Entity.Route? route = await daoRoute.getRoute(RouteID);
                // if not found
                if (route == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy tên tuyến", (int)HttpStatusCode.NotFound);
                }
                await daoRoute.DeleteRoute(route);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
