using DataAccess.DTO.IssueDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using System.Text.RegularExpressions;
using API.Model.DAO;

namespace API.Services
{
    public class IssueService
    {
        private readonly DAOIssue daoIssue = new DAOIssue();
        private async Task<List<IssueListDTO>> getList(string? filter, int page)
        {
            List<Issue> list = await daoIssue.getList(filter, page);
            List<IssueListDTO> result = new List<IssueListDTO>();
            foreach (Issue issue in list)
            {
                IssueListDTO DTO = new IssueListDTO()
                {
                    IssueId = issue.IssueId,
                    IssueName = issue.IssueName,
                    IssueCode = issue.IssueCode,
                    Description = issue.Description,
                    CreatedDate = issue.CreatedDate,
                    CableRoutingName = issue.CableRoutingName,
                    Group = issue.Group
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<IssueListDTO>> List(string? filter, int page)
        {
            List<IssueListDTO> list = await getList(filter, page);
            int RowCount = await daoIssue.getRowCount(filter);
            return new PagedResultDTO<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, list);
        }
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO, Guid CreatorID)
        {
            if(DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            if(DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            Issue issue = new Issue()
            {
                IssueId = Guid.NewGuid(),
                IssueName = DTO.IssueName.Trim(),
                IssueCode = DTO.IssueCode.Trim(),
                Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim(),
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                CreatedDate = DTO.CreatedDate,
                CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim(),
                Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim(),
                CreatorId = CreatorID,
                Status = IssueConst.STATUS_DOING,
                IsDeleted = false,
            };
            int number = await daoIssue.CreateIssue(issue);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
        }
        public async Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            Issue? issue = await daoIssue.getIssue(IssueID);
            // if not found
            if(issue == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
            }

            if (DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.NotAcceptable);
            }
            issue.IssueName = DTO.IssueName.Trim();
            issue.IssueCode = DTO.IssueCode.Trim();
            issue.Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim();
            issue.CreatedDate = DTO.CreatedDate;
            issue.CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim();
            issue.Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim();
            issue.Status = DTO.Status;
            issue.UpdateAt = DateTime.Now;
            int number = await daoIssue.UpdateIssue(issue);
            // if update successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }
        public async Task<ResponseDTO<bool>> Delete(Guid IssueID)
        {
            Issue? issue = await daoIssue.getIssue(IssueID);
            // if not found
            if (issue == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
            }
            int number = await daoIssue.DeleteIssue(issue);
            // if delete successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
