using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableDTO
{
    public class CableCreateUpdateDTO : CableImportDTO
    {
        public int SupplierId { get; set; }
        public int? YearOfManufacture { get; set; }
        public string Code { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
