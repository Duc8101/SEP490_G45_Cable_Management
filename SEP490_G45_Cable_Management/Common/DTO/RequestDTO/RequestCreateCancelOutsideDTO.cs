using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;

namespace Common.DTO.RequestDTO
{
    public class RequestCreateCancelOutsideDTO : BaseRequestCreateDTO
    {
        public RequestCreateCancelOutsideDTO()
        {
            CableCancelOutsideDTOs = new List<CableCancelOutsideDTO>();
            OtherMaterialsCancelOutsideDTOs = new List<OtherMaterialsCancelOutsideDTO>();
        }
        public Guid IssueId { get; set; }

        public List<CableCancelOutsideDTO> CableCancelOutsideDTOs { get; set; }

        public List<OtherMaterialsCancelOutsideDTO> OtherMaterialsCancelOutsideDTOs { get; set; }
    }
}
