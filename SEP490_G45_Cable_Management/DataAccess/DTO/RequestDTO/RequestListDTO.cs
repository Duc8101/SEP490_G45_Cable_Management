using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
   public class RequestListDTO
   {
        public Guid RequestId { get; set; }
        public string? RequestName { get; set; }
        public string? Content { get; set; }
        public string CreatorName { get; set; } = null!;
        public string? ApproverName { get; set; }
        public string Status { get; set; } = null!;
        //public string? RequestCategoryName { get; set; }
        public string RequestCategoryName { get; set; } = null!;
    }
}
