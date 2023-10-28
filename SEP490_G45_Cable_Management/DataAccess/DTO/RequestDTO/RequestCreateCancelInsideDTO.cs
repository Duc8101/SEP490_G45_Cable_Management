﻿using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCreateCancelInsideDTO : BaseRequestCreateDTO
    {
        public RequestCreateCancelInsideDTO()
        {
            CableCancelInsideDTOS = new List<CableCancelInsideDTO>();
            OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>();
        }
        public Guid IssueId { get; set; }

        public List<CableCancelInsideDTO> CableCancelInsideDTOS { get; set; }
        public List<OtherMaterialsExportDeliverCancelInsideDTO> OtherMaterialsCancelInsideDTOs { get; set; }

    }
}
