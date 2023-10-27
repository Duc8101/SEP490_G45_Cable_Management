using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableDTO
{
    public class CableExportDeliverDTO : CableCancelInsideDTO
    {
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
    }
}
