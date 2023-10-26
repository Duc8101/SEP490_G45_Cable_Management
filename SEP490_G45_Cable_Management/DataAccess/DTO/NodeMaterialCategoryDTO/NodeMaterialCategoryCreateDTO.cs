using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.NodeMaterialCategoryDTO
{
    public class NodeMaterialCategoryCreateDTO
    {
        public NodeMaterialCategoryCreateDTO()
        {
            NodeMaterialCategoryDTOs = new List<NodeMaterialCategoryDTO>();
        }
        public Guid NodeId { get; set; }

        public List<NodeMaterialCategoryDTO> NodeMaterialCategoryDTOs { get; set; }
    }
}
