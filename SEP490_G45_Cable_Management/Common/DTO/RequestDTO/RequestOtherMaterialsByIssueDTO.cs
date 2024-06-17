using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.RequestDTO
{
    public class RequestOtherMaterialsByIssueDTO
    {
        public string OtherMaterialsCategoryName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
