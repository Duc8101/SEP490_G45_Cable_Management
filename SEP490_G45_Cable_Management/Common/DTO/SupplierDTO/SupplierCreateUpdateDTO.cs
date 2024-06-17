using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.SupplierDTO
{
    public class SupplierCreateUpdateDTO
    {
        public string SupplierName { get; set; } = null!;
        public string? Country { get; set; }
        public string? SupplierDescription { get; set; }
    }
}
