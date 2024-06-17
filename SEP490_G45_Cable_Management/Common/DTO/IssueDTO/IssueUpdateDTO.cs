using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.IssueDTO
{
    public class IssueUpdateDTO : IssueCreateDTO
    {
        public string Status { get; set; } = null!;
    }
}
