using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestOtherMaterialsDTO
    {
        public string OtherMaterialsCategoryName { get; set; } = null!;
        public int Quantity { get; set; }
        public string? RecoveryDestWarehouseName { get; set; }
    }
}
