using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestDetailDTO : RequestListDTO
    {
        public string IssueName { get; set; } = null!;
        public string? CableRoutingName { get; set; }
    }
}
