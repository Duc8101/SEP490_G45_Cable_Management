using DataAccess.DTO.NodeDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface INodeService
    {
        Task<ResponseDTO<List<NodeListDTO>?>> List(Guid RouteID);
        Task<ResponseDTO<bool>> Create(NodeCreateDTO DTO);
        Task<ResponseDTO<NodeListDTO?>> Detail(Guid NodeID);
        Task<ResponseDTO<bool>> Update(Guid NodeID, NodeUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(Guid NodeID);
    }
}
