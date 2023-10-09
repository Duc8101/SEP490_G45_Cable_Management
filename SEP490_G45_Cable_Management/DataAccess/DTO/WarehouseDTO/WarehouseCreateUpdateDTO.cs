using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.WarehouseDTO
{
    public class WarehouseCreateUpdateDTO
    {
        //public string? WarehouseName { get; set; }
        public string WarehouseName { get; set; } = null!;
        public Guid? WarehouseKeeperId { get; set; }
        public string? WarehouseAddress { get; set; }
    }
}
