using Common.Base;
using Common.DTO.IssueDTO;
using Common.Pagination;

namespace API.Services.Issues
{
    public interface IIssueService
    {
        Task<ResponseBase<Pagination<IssueListDTO>?>> ListPagedAll(string? filter, int page);
        Task<ResponseBase<Pagination<IssueListDTO>?>> ListPagedDoing(int page);
        Task<ResponseBase<List<IssueListDTO>?>> ListDoing();
        Task<ResponseBase<bool>> Create(IssueCreateDTO DTO, Guid CreatorID);
        Task<ResponseBase<bool>> Update(Guid IssueID, IssueUpdateDTO DTO);
        Task<ResponseBase<bool>> Delete(Guid IssueID);
        Task<ResponseBase<List<IssueDetailDTO>?>> Detail(Guid IssueID);
    }
}
