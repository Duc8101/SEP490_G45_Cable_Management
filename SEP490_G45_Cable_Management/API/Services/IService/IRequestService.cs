using DataAccess.DTO;
using DataAccess.DTO.RequestDTO;

namespace API.Services.IService
{
    public interface IRequestService
    {
        Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, Guid? CreatorID, int page);
        Task<ResponseDTO<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Approve(Guid RequestID, Guid ApproverID, string ApproverName);
        Task<ResponseDTO<bool>> Reject(Guid RequestID, Guid RejectorID);
        Task<ResponseDTO<bool>> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Delete(Guid RequestID);
        Task<ResponseDTO<RequestDetailDTO?>> Detail(Guid RequestID);
    }
}
