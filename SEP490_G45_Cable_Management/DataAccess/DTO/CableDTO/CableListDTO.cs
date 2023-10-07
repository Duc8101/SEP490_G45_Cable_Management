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
        public string? WarehouseName { get; set; }
        public string? SupplierName { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public int? YearOfManufacture { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; } = null!;
        public string CableCategoryName { get; set; } = null!;
    }
}
