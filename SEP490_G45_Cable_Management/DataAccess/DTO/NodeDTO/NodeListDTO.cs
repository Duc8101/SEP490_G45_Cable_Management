using DataAccess.DTO.NodeMaterialCategoryDTO;
using DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.NodeDTO
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
