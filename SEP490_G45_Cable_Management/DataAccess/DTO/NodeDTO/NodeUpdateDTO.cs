﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.NodeDTO
{
    public class NodeUpdateDTO
    {
        public string NodeCode { get; set; } = null!;
        public string NodeNumberSign { get; set; } = null!;
        public string? Address { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
    }
}
