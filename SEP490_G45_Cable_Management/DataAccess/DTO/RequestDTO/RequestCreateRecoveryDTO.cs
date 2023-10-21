﻿using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCreateRecoveryDTO : BaseRequestCreateDTO
    {
        public RequestCreateRecoveryDTO()
        {
            CableCreateUpdateDTOs = new List<CableCreateUpdateDTO>();
            OtherMaterialsRecoveryDTOs = new List<OtherMaterialsRecoveryDTO>();
        }
        public Guid IssueId { get; set; }
        public List<CableCreateUpdateDTO> CableCreateUpdateDTOs { get; set; }

        public List<OtherMaterialsRecoveryDTO> OtherMaterialsRecoveryDTOs { get; set; }
    }
}
