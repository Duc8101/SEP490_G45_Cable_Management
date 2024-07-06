using Common.Base;
using Common.DTO.NodeDTO;

namespace API.Services.Nodes
{
    public interface INodeService
    {
        ResponseBase List(Guid routeId);
        ResponseBase Create(NodeCreateDTO DTO);
        ResponseBase Detail(Guid nodeId);
        ResponseBase Update(Guid nodeId, NodeUpdateDTO DTO);
        ResponseBase Delete(Guid nodeId);
    }
}
