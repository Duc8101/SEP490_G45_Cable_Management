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
        public string RequestName { get; set; } = null!;
        public string? Content { get; set; }
        public Guid CreatorId { get; set; }
        public Guid? ApproverId { get; set; }
        public string Status { get; set; } = null!;

        public string RequestCategoryName { get; set; } = null!;
    }
}
