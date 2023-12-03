using API.Model.DAO;
using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeDTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.Entity;
using System.Net;

namespace API.Services.Service
{
    public class NodeService : INodeService
    {
        private readonly DAONode daoNode = new DAONode();
        private readonly DAONodeMaterialCategory daoCategory = new DAONodeMaterialCategory();
        private async Task<List<NodeMaterialCategoryListDTO>> getListNodeCategory(Guid NodeID)
        {
            List<NodeMaterialCategory> NodeMaterialCategories = await daoCategory.getList(NodeID);
            List<NodeMaterialCategoryListDTO> list = new List<NodeMaterialCategoryListDTO>();
            foreach (NodeMaterialCategory item in NodeMaterialCategories)
            {
                NodeMaterialCategoryListDTO categoryDTO = new NodeMaterialCategoryListDTO()
                {
                    OtherMaterialsCategoryName = item.OtherMaterialCategory.OtherMaterialsCategoryName,
                    Quantity = item.Quantity,
                };
                list.Add(categoryDTO);
            }
            return list;
        }
        private async Task<List<NodeListDTO>> getListNode(Guid RouteID)
        {
            List<Node> list = await daoNode.getListNotDeleted(RouteID);
            List<NodeListDTO> result = new List<NodeListDTO>();
            foreach (Node node in list)
            {
                List<NodeMaterialCategoryListDTO> categories = await getListNodeCategory(node.Id);
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
                    Note = node.Note,
                    NumberOrder = node.NumberOrder,
                    //NodeCables = (List<NodeCable>)node.NodeCables,
                    //NodeMaterials = (List<NodeMaterial>)node.NodeMaterials,
                    NodeMaterialCategoryListDTOs = categories
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<List<NodeListDTO>?>> List(Guid RouteID)
        {
            try
            {
                List<NodeListDTO> list = await getListNode(RouteID);
                return new ResponseDTO<List<NodeListDTO>?>(list, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<NodeListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        private async Task UpdateNodeDeleted(Guid RouteID)
        {
            List<Node> list = await daoNode.getListDeleted(RouteID);
            if (list.Count > 0)
            {
                foreach (Node node in list)
                {
                    node.RouteId = null;
                    await daoNode.UpdateNode(node);
                }
            }
        }
        public async Task<ResponseDTO<bool>> Create(NodeCreateDTO DTO)
        {
            if (DTO.RouteId == null)
            {
                return new ResponseDTO<bool>(false, "Bạn chưa chọn tuyến", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // update node deleted
                await UpdateNodeDeleted(DTO.RouteId.Value);
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
                Node node = new Node()
                {
                    Id = Guid.NewGuid(),
                    Longitude = DTO.Longitude,
                    Latitude = DTO.Latitude,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    Address = DTO.Address == null || DTO.Address.Trim().Length == 0 ? null : DTO.Address.Trim(),
                    NodeCode = DTO.NodeCode.Trim(),
                    NodeNumberSign = DTO.NodeNumberSign.Trim(),
                    Note = DTO.Note == null || DTO.Note.Trim().Length == 0 ? null : DTO.Note.Trim(),
                    NumberOrder = DTO.NumberOrder,
                    RouteId = DTO.RouteId,
                    Status = DTO.Status == null || DTO.Status.Trim().Length == 0 ? null : DTO.Status.Trim(),
                };
                await daoNode.CreateNode(node);
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }
        public async Task<ResponseDTO<NodeListDTO?>> Detail(Guid NodeID)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                // if not found
                if (node == null)
                {
                    return new ResponseDTO<NodeListDTO?>(null, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                List<NodeMaterialCategoryListDTO> list = await getListNodeCategory(NodeID);
                NodeListDTO nodeDTO = new NodeListDTO()
                {
                    Id = node.Id,
                    NodeCode = node.NodeCode,
                    NodeNumberSign = node.NodeNumberSign,
                    Address = node.Address,
                    Longitude = node.Longitude,
                    Latitude = node.Latitude,
                    Status = node.Status,
                    RouteId = node.RouteId,
                    Note = node.Note,
                    NumberOrder = node.NumberOrder,
                    //NodeCables = (List<NodeCable>)node.NodeCables,
                    //NodeMaterials = (List<NodeMaterial>)node.NodeMaterials,
                    NodeMaterialCategoryListDTOs = list
                };
                return new ResponseDTO<NodeListDTO?>(nodeDTO, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<NodeListDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(Guid NodeID, NodeUpdateDTO DTO)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                if (node == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
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
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(Guid NodeID)
        {
            try
            {
                Node? node = await daoNode.getNode(NodeID);
                if (node == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound);
                }
                await daoNode.DeleteNode(node);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
