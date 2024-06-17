using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.RequestDTO
{
    public class RequestCreateDeliverDTO : BaseRequestCreateDTO
    {
        public RequestCreateDeliverDTO()
        {
            CableDeliverDTOs = new List<CableExportDeliverDTO>();
            OtherMaterialsDeliverDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>();
        }
        public int DeliverWareHouseID { get; set; }
        public List<CableExportDeliverDTO> CableDeliverDTOs { get; set; }

        public List<OtherMaterialsExportDeliverCancelInsideDTO> OtherMaterialsDeliverDTOs { get; set; }

    }
}
