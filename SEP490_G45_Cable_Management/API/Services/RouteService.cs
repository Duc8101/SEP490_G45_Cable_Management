using DataAccess.DTO;
using DataAccess.DTO.RouteDTO;
using System.Net;
using API.Model;

namespace API.Services
{
    public class RouteService
    {
        private readonly DAORoute daoRoute = new DAORoute();
        public async Task<List<RouteListDTO>> List(string? name)
        {
            List<DataAccess.Entity.Route> list = await daoRoute.getList(name);
            List<RouteListDTO> result = new List<RouteListDTO>();
            foreach(DataAccess.Entity.Route route in list)
            {
                RouteListDTO DTO = new RouteListDTO()
                {
                    RouteId = route.RouteId,
                    RouteName = route.RouteName,
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<ResponseDTO<bool>> Create(RouteCreateDTO DTO)
        {
            if(DTO.RouteName == null || DTO.RouteName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên tuyến không được để trống", (int)HttpStatusCode.NotAcceptable);

            }
            // if exist
            if (await daoRoute.isExist(DTO.RouteName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Tên tuyến đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            DataAccess.Entity.Route route = new DataAccess.Entity.Route()
            {
                RouteId = Guid.NewGuid(),
                RouteName = DTO.RouteName.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            int number = await daoRoute.CreateRoute(route);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(Guid RouteID)
        {
            DataAccess.Entity.Route? route = await daoRoute.getRoute(RouteID);
            // if not found
            if(route == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy tên tuyến", (int) HttpStatusCode.NotFound);
            }
            int number = await daoRoute.DeleteRoute(route);
            // if delete successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
