using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
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
