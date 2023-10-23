using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.NodeDTO
{
    public class NodeCreateDTO : NodeUpdateDTO
    {
        public Guid? RouteId { get; set; }
        public int NumberOrder { get; set; }

    }
}
