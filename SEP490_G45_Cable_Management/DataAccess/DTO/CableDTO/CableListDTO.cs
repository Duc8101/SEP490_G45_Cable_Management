using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableDTO
{
    public class CableListDTO
    {
        public Guid CableId { get; set; }
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public int SupplierId { get; set; }
        //public string? SupplierName { get; set; }
        public string SupplierName { get; set; } = null!;
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public int? YearOfManufacture { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; } = null!;
        public int CableCategoryId { get; set; }
        public string CableCategoryName { get; set; } = null!;
        public bool IsExportedToUse { get; set; }
        public bool IsInRequest { get; set; }
    }
}
