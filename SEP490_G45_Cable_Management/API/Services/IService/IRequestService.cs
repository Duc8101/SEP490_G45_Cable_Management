﻿using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.DTO.CableDTO;

namespace API.Services.IService
{
    public interface IRequestService
    {
        Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, string? status, Guid? CreatorID, int page);
        Task<ResponseDTO<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Approve(Guid RequestID, Guid ApproverID, string ApproverName);
        Task<ResponseDTO<bool>> Reject(Guid RequestID, Guid RejectorID);
        Task<ResponseDTO<List<CableListDTO>?>> SuggestionCable(SuggestionCableDTO suggestion);
    }
}
