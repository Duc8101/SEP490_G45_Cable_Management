﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OtherMaterialsDTO
{
    public class OtherMaterialsCancelOutsideDTO
    {
        public string Unit { get; set; } = null!;
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public string? Status { get; set; }
        public int OtherMaterialsCategoryId { get; set; }
    }
}
