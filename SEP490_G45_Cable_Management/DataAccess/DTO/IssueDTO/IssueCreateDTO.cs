using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.IssueDTO
{
    public class IssueCreateDTO
    {
        public string? IssueName { get; set; }
        public string? IssueCode { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CableRoutingName { get; set; }
        public string? Group { get; set; }

    }
}
