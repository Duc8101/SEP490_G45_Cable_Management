using DataAccess.DTO.IssueDTO;
using DataAccess.DTO;

namespace API.Services.IService
{
    public interface IIssueService
    {
        Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedAll(string? filter, int page);
        Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedDoing(int page);
        Task<ResponseDTO<List<IssueListDTO>?>> ListDoing();
        Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO, Guid CreatorID);
        Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO);
        Task<ResponseDTO<bool>> Delete(Guid IssueID);
        Task<ResponseDTO<List<IssueDetailDTO>?>> Detail(Guid IssueID);
    }
}
