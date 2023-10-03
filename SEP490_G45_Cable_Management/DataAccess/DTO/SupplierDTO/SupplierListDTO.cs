using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.SupplierDTO
{
    public class SupplierListDTO : SupplierCreateUpdateDTO
    {
        public int SupplierId { get; set; }
    }
}
