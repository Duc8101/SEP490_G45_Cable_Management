using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.RequestDTO
{
    public class RequestCreateExportDTO : BaseRequestCreateDTO
    {
        public RequestCreateExportDTO()
        {
            CableExportDTOs = new List<CableExportDeliverDTO>();
            OtherMaterialsExportDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>();
        }

        public Guid IssueId { get; set; }

        public List<CableExportDeliverDTO> CableExportDTOs { get; set; }

        public List<OtherMaterialsExportDeliverCancelInsideDTO> OtherMaterialsExportDTOs { get; set; }

    }
}
