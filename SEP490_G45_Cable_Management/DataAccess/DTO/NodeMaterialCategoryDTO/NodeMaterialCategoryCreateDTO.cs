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
            MaterialCategoryDTOs = new List<MaterialCategoryDTO>();
        }
        public Guid NodeId { get; set; }

        public List<MaterialCategoryDTO> MaterialCategoryDTOs { get; set; }
    }
}
