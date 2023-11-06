using DataAccess.DTO.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.IssueDTO
{
    public class IssueDetailDTO
    {
        public IssueDetailDTO()
        {
            RequestCableDTOs = new List<RequestCableByIssueDTO>();
            RequestOtherMaterialsDTOs = new List<RequestOtherMaterialsByIssueDTO>();
        }
        public string RequestName { get; set; } = null!;
        public string? ApproverName { get; set; }
        public string? CableRoutingName { get; set; }
        public string? Group { get; set; }
        public List<RequestCableByIssueDTO> RequestCableDTOs { get; set; }
        public List<RequestOtherMaterialsByIssueDTO> RequestOtherMaterialsDTOs { get; set; }
    }
}
