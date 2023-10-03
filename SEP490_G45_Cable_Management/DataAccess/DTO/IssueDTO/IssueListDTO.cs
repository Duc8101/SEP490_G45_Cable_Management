using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.IssueDTO
{
    public class IssueListDTO : IssueUpdateDTO
    {
        public Guid IssueId { get; set; }
    }
}
