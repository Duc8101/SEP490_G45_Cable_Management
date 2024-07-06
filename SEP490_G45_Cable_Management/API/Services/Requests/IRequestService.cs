using Common.Base;
using Common.DTO.RequestDTO;

namespace API.Services.Requests
{
    public interface IRequestService
    {
        ResponseBase List(string? name, int? requestCategoryId, string? status, Guid? creatorId, int page);
        Task<ResponseBase> CreateRequestExport(RequestCreateExportDTO DTO, Guid creatorId);
        Task<ResponseBase> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid creatorId);
        Task<ResponseBase> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid creatorId);
        Task<ResponseBase> Approve(Guid requestId, Guid approverId, string approverName);
        ResponseBase Reject(Guid requestId, Guid rejectorId);
        Task<ResponseBase> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid creatorId);
        Task<ResponseBase> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid creatorId);
        ResponseBase Delete(Guid requestId);
        ResponseBase Detail(Guid requestId);
    }
}
