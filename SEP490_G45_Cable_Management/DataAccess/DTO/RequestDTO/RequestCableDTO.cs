﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCableDTO
    {
        public string CableCategoryName { get; set; } = null!;
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public string? RecoveryDestWarehouseName { get; set; }
    }
}
