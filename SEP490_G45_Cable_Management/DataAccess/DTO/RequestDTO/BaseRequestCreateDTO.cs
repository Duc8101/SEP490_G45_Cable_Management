using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class BaseRequestCreateDTO
    {
        public string RequestName { get; set; } = null!;
        public string? Content { get; set; }
        public int RequestCategoryId { get; set; }
    }
}
