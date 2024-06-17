using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.RequestDTO
{
    public class RequestCreateCancelInsideDTO : BaseRequestCreateDTO
    {
        public RequestCreateCancelInsideDTO()
        {
            CableCancelInsideDTOs = new List<CableCancelInsideDTO>();
            OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>();
        }
        public Guid IssueId { get; set; }

        public List<CableCancelInsideDTO> CableCancelInsideDTOs { get; set; }
        public List<OtherMaterialsExportDeliverCancelInsideDTO> OtherMaterialsCancelInsideDTOs { get; set; }

    }
}
