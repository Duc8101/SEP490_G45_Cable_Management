﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableDTO
{
    public class CableExportDTO
    {
        public int WarehouseId { get; set; }
        public Guid CableId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
    }
}