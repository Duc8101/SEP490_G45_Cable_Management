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
        public string? Unit { get; set; }
        public int? Quantity { get; set; }
        public string? Code { get; set; }
        //public string? SupplierName { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? WarehouseName { get; set; }
        public string? OtherMaterialsCategoryName { get; set; }
    }
}
