﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCableListDTO : RequestCableByIssueDTO
    {
        public string? RecoveryDestWarehouseName { get; set; }
    }
}
