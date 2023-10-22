using API.Model.DAO;
using DataAccess.DTO;
using DataAccess.DTO.NodeDTO;
using DataAccess.Entity;
using System.Collections.Generic;
using System.Net;

namespace API.Services
{
    public class NodeService
    {
        private readonly DAONode daoNode = new DAONode();
        private async Task<List<NodeListDTO>> getList(Guid RouteID)
        {
            List<Node> list = await daoNode.getList(RouteID);
            List<NodeListDTO> result = new List<NodeListDTO>();
            foreach (Node node in list)
            {
                NodeListDTO DTO = new NodeListDTO()
                {
                    Id = node.Id,
                    NodeCode = node.NodeCode,
                    NodeNumberSign = node.NodeNumberSign,
                    Address = node.Address,
                    Longitude = node.Longitude,
                    Latitude = node.Latitude,
                    Status = node.Status,
                    RouteId = node.RouteId,
                    NodeCables = (List<NodeCable>) node.NodeCables,
                    NodeMaterials = (List<NodeMaterial>) node.NodeMaterials,
                    NodeMaterialCategories = (List<NodeMaterialCategory>) node.NodeMaterialCategories
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<List<NodeListDTO>?>> List(Guid RouteID)
        {
            try
            {
                List<NodeListDTO> list = await getList(RouteID);
                return new ResponseDTO<List<NodeListDTO>?>(list, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<NodeListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
