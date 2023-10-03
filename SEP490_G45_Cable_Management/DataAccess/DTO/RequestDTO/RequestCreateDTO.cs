using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.RequestDTO
{
    public class RequestCreateDTO
    {
        public RequestCreateDTO()
        {
            CableImportDTOs = new List<CableImportDTO>();
            OtherMaterialsImportDTOs = new List<OtherMaterialsImportDTO>();
        }
        public string RequestName { get; set; } = null!;
        public string? Content { get; set; }
        public Guid? IssueId { get; set; }
        public int RequestCategoryId { get; set; }
        public int? DeliverWarehouseId { get; set; }
        public List<CableImportDTO> CableImportDTOs { get; set; }
        public List<OtherMaterialsImportDTO> OtherMaterialsImportDTOs { get; set; }
    }
}
