using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OtherMaterialsDTO
{
    public class OtherMaterialsListDTO
    {
        public int OtherMaterialsId { get; set; }
        public string Unit { get; set; } = null!;
        public int Quantity { get; set; }
        public string Code { get; set; } = null!;
        public string SupplierName { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public string OtherMaterialsCategoryName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
