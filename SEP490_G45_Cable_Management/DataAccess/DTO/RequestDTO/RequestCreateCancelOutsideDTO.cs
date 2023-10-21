using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCreateCancelOutsideDTO : BaseRequestCreateDTO
    {
        public RequestCreateCancelOutsideDTO()
        {
            CableCancelOutsideDTOs = new List<CableCancelOutsideDTO>();
            OtherMaterialsCancelOutsideDTOs = new List<OtherMaterialsCancelOutsideDTO>();
        }
        public Guid IssueId { get; set; }

        public List<CableCancelOutsideDTO> CableCancelOutsideDTOs { get; set; }

        public List<OtherMaterialsCancelOutsideDTO> OtherMaterialsCancelOutsideDTOs { get; set; }
    }
}
