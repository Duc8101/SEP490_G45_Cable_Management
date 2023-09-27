using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableCategoryDTO
{
    public class CableCategoryListDTO : CableCategoryCreateUpdateDTO
    {
        public int CableCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
