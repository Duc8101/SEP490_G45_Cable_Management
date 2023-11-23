using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.NodeMaterialCategoryDTO
{
    public class NodeMaterialCategoryUpdateDTO
    {
        public NodeMaterialCategoryUpdateDTO()
        {
            MaterialCategoryDTOs = new List<MaterialCategoryDTO>();
        }

        public List<MaterialCategoryDTO> MaterialCategoryDTOs { get; set; }
    }
}
