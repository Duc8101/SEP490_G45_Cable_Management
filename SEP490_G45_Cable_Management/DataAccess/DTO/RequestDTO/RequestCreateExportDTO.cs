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
            CableExportDTOs = new List<CableExportDTO>();
            OtherMaterialsExportDTOs = new List<OtherMaterialsExportDTO>();
        }

        public Guid IssueId { get; set; }

        public List<CableExportDTO> CableExportDTOs { get; set; }

        public List<OtherMaterialsExportDTO> OtherMaterialsExportDTOs { get; set; }

    }
}
