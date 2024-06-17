using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.CableDTO
{
    public class CableCreateUpdateDTO : CableCancelOutsideDTO
    {
        public int WarehouseId { get; set; }
    }
}
