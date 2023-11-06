using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestDetailDTO : RequestListDTO
    {
        public RequestDetailDTO()
        {
            RequestCableDTOs = new List<RequestCableListDTO>();
            RequestOtherMaterialsDTOs = new List<RequestOtherMaterialsListDTO>();
        }
        public string? IssueName { get; set; }
        public string? CableRoutingName { get; set; }
        public string? DeliverWarehouseName { get; set; }
        public List<RequestCableListDTO> RequestCableDTOs { get; set; }
        public List<RequestOtherMaterialsListDTO> RequestOtherMaterialsDTOs { get; set; }

    }
}
