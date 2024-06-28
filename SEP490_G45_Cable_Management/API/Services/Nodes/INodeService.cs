using Common.Base;
using Common.DTO.NodeDTO;

namespace API.Services.Nodes
{
    public interface INodeService
    {
        Task<ResponseBase<List<NodeListDTO>?>> List(Guid RouteID);
        Task<ResponseBase<bool>> Create(NodeCreateDTO DTO);
        Task<ResponseBase<NodeListDTO?>> Detail(Guid NodeID);
        Task<ResponseBase<bool>> Update(Guid NodeID, NodeUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(Guid NodeID);
    }
}
