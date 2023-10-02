using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RouteDTO
{
    public class RouteListDTO : RouteCreateDTO
    {
        public Guid RouteId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
