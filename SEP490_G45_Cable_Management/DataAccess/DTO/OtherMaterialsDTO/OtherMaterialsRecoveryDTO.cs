﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.OtherMaterialsDTO
{
    public class OtherMaterialsRecoveryDTO : OtherMaterialsCreateUpdateDTO
    {
        public int SupplierId { get; set; }
    }
}
