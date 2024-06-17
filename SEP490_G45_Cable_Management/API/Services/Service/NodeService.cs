using API.Services.IService;
using AutoMapper;
using Common.Base;
using Common.DTO.NodeDTO;
using Common.DTO.NodeMaterialCategoryDTO;
using Common.Entity;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Service
{
    public class NodeService : BaseService, INodeService
    {
        private readonly DAONode daoNode = new DAONode();
        private readonly DAONodeMaterialCategory daoCategory = new DAONodeMaterialCategory();
        private readonly DAORoute daoRoute = new DAORoute();

        public NodeService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<List<NodeListDTO>?>> List(Guid RouteID)
        {
            try
            {
                List<Node> list = await daoNode.getList(RouteID);
                List<NodeListDTO> data = _mapper.Map<List<NodeListDTO>>(list);
                foreach (NodeListDTO item in data)
                {
                    List<NodeMaterialCategory> NodeMaterialCategories = await daoCategory.getList(item.Id);
                    List<NodeMaterialCategoryListDTO> categories = _mapper.Map<List<NodeMaterialCategoryListDTO>>(NodeMaterialCategories);
                    item.NodeMaterialCategoryListDTOs = categories;
                }
                return new ResponseBase<List<NodeListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<NodeListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(NodeCreateDTO DTO)
        {
            if (DTO.RouteId == null)
            {
                return new ResponseBase<bool>(false, "Bạn chưa chọn tuyến", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Common.Entity.Route? route = await daoRoute.getRoute(DTO.RouteId.Value);
                if (route == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy tuyến", (int)HttpStatusCode.NotFound);
                }
                // --------------------------- update list node order by ---------------------------
                List<Node> list = await daoNode.getListNodeOrderByNumberOrder(DTO.RouteId.Value);
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
                            await daoNode.UpdateNode(list[i]);
                        }
                    }
                }
                list = await daoNode.getListNodeOrderByNumberOrder(DTO.RouteId.Value);
                if (list.Count > 0)
                {
                    foreach (Node item in list)
                    {
                        if (item.NumberOrder >= DTO.NumberOrder)
                        {
                            item.NumberOrder++;
                            await daoNode.UpdateNode(item);
                        }
                    }
                }
                // --------------------------- create node ---------------------------
                Node node = _mapper.Map<Node>(DTO);
                node.Id = Guid.NewGuid();
                node.CreatedAt = DateTime.Now;
                node.UpdateAt = DateTime.Now;
                node.IsDeleted = false;
                await daoNode.CreateNode(node);
                return new ResponseBase<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }
        public async Task<ResponseBase<NodeListDTO?>> Detail(Guid NodeID)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                // if not found
                if (node == null)
                {
                    return new ResponseBase<NodeListDTO?>(null, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                List<NodeMaterialCategory> NodeMaterialCategories = await daoCategory.getList(NodeID);
                List<NodeMaterialCategoryListDTO> list = _mapper.Map<List<NodeMaterialCategoryListDTO>>(NodeMaterialCategories);
                NodeListDTO DTO = _mapper.Map<NodeListDTO>(node);
                DTO.NodeMaterialCategoryListDTOs = list;
                return new ResponseBase<NodeListDTO?>(DTO, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<NodeListDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(Guid NodeID, NodeUpdateDTO DTO)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                if (node == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                node.Longitude = DTO.Longitude;
                node.Latitude = DTO.Latitude;
                node.UpdateAt = DateTime.Now;
                node.Address = DTO.Address == null || DTO.Address.Trim().Length == 0 ? null : DTO.Address.Trim();
                node.NodeCode = DTO.NodeCode.Trim();
                node.NodeNumberSign = DTO.NodeNumberSign.Trim();
                node.Note = DTO.Note == null || DTO.Note.Trim().Length == 0 ? null : DTO.Note.Trim();
                node.Status = DTO.Status == null || DTO.Status.Trim().Length == 0 ? null : DTO.Status.Trim();
                await daoNode.UpdateNode(node);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(Guid NodeID)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                if (node == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                await daoNode.DeleteNode(node);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
