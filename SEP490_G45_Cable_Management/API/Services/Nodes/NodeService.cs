using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.NodeDTO;
using Common.DTO.NodeMaterialCategoryDTO;
using Common.Entity;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Nodes
{
    public class NodeService : BaseService, INodeService
    {
        private readonly DAONode _daoNode;
        private readonly DAONodeMaterialCategory _daoNodeMaterialCategory;
        private readonly DAORoute _daoRoute;
        public NodeService(IMapper mapper, DAONode daoNode, DAONodeMaterialCategory daoNodeMaterialCategory, DAORoute daoRoute) : base(mapper)
        {
            _daoNode = daoNode;
            _daoNodeMaterialCategory = daoNodeMaterialCategory;
            _daoRoute = daoRoute;
        }

        public ResponseBase Create(NodeCreateDTO DTO)
        {
            if (DTO.RouteId == null)
            {
                return new ResponseBase(false, "Bạn chưa chọn tuyến", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Common.Entity.Route? route = _daoRoute.getRoute(DTO.RouteId.Value);
                if (route == null)
                {
                    return new ResponseBase(false, "Không tìm thấy tuyến", (int)HttpStatusCode.NotFound);
                }
                // --------------------------- update list node order by ---------------------------
                List<Node> list = _daoNode.getListNode(DTO.RouteId.Value, false);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        // if number order not correct
                        if (list[i].NumberOrder != i + 1)
                        {
                            // if find point position
                            if (DTO.NumberOrder - 1 == list[i].NumberOrder)
                            {
                                list[i].NumberOrder = i + 1;
                                DTO.NumberOrder = list[i].NumberOrder + 1;
                            }
                            else
                            {
                                list[i].NumberOrder = i + 1;
                            }
                            _daoNode.UpdateNode(list[i]);
                        }
                    }
                }
                list = _daoNode.getListNode(DTO.RouteId.Value, false);
                if (list.Count > 0)
                {
                    foreach (Node item in list)
                    {
                        if (item.NumberOrder >= DTO.NumberOrder)
                        {
                            item.NumberOrder++;
                            _daoNode.UpdateNode(item);
                        }
                    }
                }
                // --------------------------- create node ---------------------------
                Node node = _mapper.Map<Node>(DTO);
                node.Id = Guid.NewGuid();
                node.CreatedAt = DateTime.Now;
                node.UpdateAt = DateTime.Now;
                node.IsDeleted = false;
                _daoNode.CreateNode(node);
                return new ResponseBase(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(Guid nodeId)
        {
            try
            {
                Node? node = _daoNode.getNode(nodeId);
                if (node == null)
                {
                    return new ResponseBase(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                _daoNode.DeleteNode(node);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Detail(Guid nodeId)
        {
            try
            {
                Node? node = _daoNode.getNode(nodeId);
                // if not found
                if (node == null)
                {
                    return new ResponseBase("Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                List<NodeMaterialCategory> list = _daoNodeMaterialCategory.getListNodeMaterialCategory(nodeId);
                List<NodeMaterialCategoryListDTO> DTO = _mapper.Map<List<NodeMaterialCategoryListDTO>>(list);
                NodeListDTO data = _mapper.Map<NodeListDTO>(node);
                data.NodeMaterialCategoryListDTOs = DTO;
                return new ResponseBase(DTO);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase List(Guid routeId)
        {
            try
            {
                List<Node> listNode = _daoNode.getListNode(routeId, true);
                List<NodeListDTO> data = _mapper.Map<List<NodeListDTO>>(listNode);
                foreach (NodeListDTO item in data)
                {
                    List<NodeMaterialCategory> listNodeMaterialCategory = _daoNodeMaterialCategory.getListNodeMaterialCategory(item.Id);
                    List<NodeMaterialCategoryListDTO> DTO = _mapper.Map<List<NodeMaterialCategoryListDTO>>(listNodeMaterialCategory);
                    item.NodeMaterialCategoryListDTOs = DTO;
                }
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(Guid nodeId, NodeUpdateDTO DTO)
        {
            try
            {
                Node? node = _daoNode.getNode(nodeId);
                if (node == null)
                {
                    return new ResponseBase(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                node.Longitude = DTO.Longitude;
                node.Latitude = DTO.Latitude;
                node.UpdateAt = DateTime.Now;
                node.Address = DTO.Address == null || DTO.Address.Trim().Length == 0 ? null : DTO.Address.Trim();
                node.NodeCode = DTO.NodeCode.Trim();
                node.NodeNumberSign = DTO.NodeNumberSign.Trim();
                node.Note = DTO.Note == null || DTO.Note.Trim().Length == 0 ? null : DTO.Note.Trim();
                node.Status = DTO.Status == null || DTO.Status.Trim().Length == 0 ? null : DTO.Status.Trim();
                _daoNode.UpdateNode(node);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
