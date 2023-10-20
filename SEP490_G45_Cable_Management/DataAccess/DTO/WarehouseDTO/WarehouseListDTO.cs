using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.WarehouseDTO
{
    public class WarehouseListDTO : WarehouseCreateUpdateDTO
    {
        public int WarehouseId { get; set; }

        public string? WareWarehouseKeeperName { get; set; }
    }
}
