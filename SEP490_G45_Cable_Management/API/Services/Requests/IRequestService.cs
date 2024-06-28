using Common.Base;
using Common.DTO.RequestDTO;
using Common.Pagination;

namespace API.Services.Requests
{
    public interface IRequestService
    {
        Task<ResponseBase<Pagination<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, Guid? CreatorID, int page);
        Task<ResponseBase<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Approve(Guid RequestID, Guid ApproverID, string ApproverName);
        Task<ResponseBase<bool>> Reject(Guid RequestID, Guid RejectorID);
        Task<ResponseBase<bool>> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Delete(Guid RequestID);
        Task<ResponseBase<RequestDetailDTO?>> Detail(Guid RequestID);
    }
}
