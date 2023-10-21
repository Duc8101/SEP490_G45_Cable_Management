using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCreateDeliverDTO : BaseRequestCreateDTO
    {
        public RequestCreateDeliverDTO()
        {
            CableExportDTOs = new List<CableExportDTO>();
            OtherMaterialsExportDTOs = new List<OtherMaterialsExportCancelInsideDTO>();
        }
        public int DeliverWareHouseID { get; set; }
        public List<CableExportDTO> CableExportDTOs { get; set; }

        public List<OtherMaterialsExportCancelInsideDTO> OtherMaterialsExportDTOs { get; set; }

    }
}
