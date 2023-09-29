using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OtherMaterialsCategoryDTO
{
    public class OtherMaterialsCategoryListDTO : OtherMaterialsCategoryCreateUpdateDTO
    {
        public int OtherMaterialsCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
