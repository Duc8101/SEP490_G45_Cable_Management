using Common.Base;
using Common.DTO.IssueDTO;

namespace API.Services.Issues
{
    public interface IIssueService
    {
        ResponseBase ListPagedAll(string? filter, int page);
        ResponseBase ListPagedDoing(int page);
        ResponseBase ListDoing();
        ResponseBase Create(IssueCreateDTO DTO, Guid creatorId);
        ResponseBase Update(Guid issueId, IssueUpdateDTO DTO);
        ResponseBase Delete(Guid issueId);
        ResponseBase Detail(Guid issueId);
    }
}
