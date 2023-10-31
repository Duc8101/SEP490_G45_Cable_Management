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
            RequestCableDTOs = new List<RequestCableDTO>();
            RequestOtherMaterialsDTOs = new List<RequestOtherMaterialsDTO>();
        }
        public string? IssueName { get; set; }
        public string? CableRoutingName { get; set; }
        public string? DeliverWarehouseName { get; set; }
        public List<RequestCableDTO> RequestCableDTOs { get; set; }
        public List<RequestOtherMaterialsDTO> RequestOtherMaterialsDTOs { get; set; }

    }
}
