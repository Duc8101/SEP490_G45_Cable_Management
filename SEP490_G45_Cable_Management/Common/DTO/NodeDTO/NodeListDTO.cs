using Common.DTO.NodeMaterialCategoryDTO;

namespace Common.DTO.NodeDTO
{
    public class NodeListDTO : NodeCreateDTO
    {
        public NodeListDTO()
        {
            //NodeCables = new List<NodeCable>();
            // NodeMaterials = new List<NodeMaterial>();
            NodeMaterialCategoryListDTOs = new List<NodeMaterialCategoryListDTO>();
        }
        public Guid Id { get; set; }
        //public List<NodeCable> NodeCables { get; set; }
        //public List<NodeMaterial> NodeMaterials { get; set; }
        public List<NodeMaterialCategoryListDTO> NodeMaterialCategoryListDTOs { get; set; }

    }
}
