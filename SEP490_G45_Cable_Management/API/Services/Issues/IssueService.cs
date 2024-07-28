using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.IssueDTO;
using Common.DTO.RequestDTO;
using Common.Entity;
using Common.Enums;
using Common.Paginations;
using DataAccess.DAO;
using DataAccess.Extensions;
using System.Net;

namespace API.Services.Issues
{
    public class IssueService : BaseService, IIssueService
    {
        private readonly DAOIssue _daoIssue;
        private readonly DAORequestCable _daoRequestCable;
        private readonly DAORequestOtherMaterial _daoRequestOtherMaterial;
        public IssueService(IMapper mapper, DAOIssue daoIssue, DAORequestCable daoRequestCable, DAORequestOtherMaterial daoRequestOtherMaterial) : base(mapper)
        {
            _daoIssue = daoIssue;
            _daoRequestCable = daoRequestCable;
            _daoRequestOtherMaterial = daoRequestOtherMaterial;
        }

        public ResponseBase Create(IssueCreateDTO DTO, Guid creatorId)
        {
            if (DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseBase(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }
            Issue issue = _mapper.Map<Issue>(DTO);
            issue.IssueId = Guid.NewGuid();
            issue.CreatedAt = DateTime.Now;
            issue.UpdateAt = DateTime.Now;
            issue.CreatorId = creatorId;
            issue.Status = IssueStatus.Doing.getDescription();
            issue.IsDeleted = false;
            try
            {
                _daoIssue.CreateIssue(issue);
                return new ResponseBase(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(Guid issueId)
        {
            try
            {
                Issue? issue = _daoIssue.getIssue(issueId);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                _daoIssue.DeleteIssue(issue);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Detail(Guid issueId)
        {
            try
            {
                Issue? issue = _daoIssue.getIssue(issueId);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase("Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                List<Request> requests = issue.Requests.Where(r => r.IsDeleted == false).ToList();
                List<IssueDetailDTO> data = new List<IssueDetailDTO>();
                foreach (Request request in requests)
                {
                    List<RequestCable> requestCables = _daoRequestCable.getListRequestCable(request.RequestId);
                    List<RequestCableByIssueDTO> requestCableDTOs = _mapper.Map<List<RequestCableByIssueDTO>>(requestCables);
                    List<RequestOtherMaterial> requestMaterials = _daoRequestOtherMaterial.getListRequestOtherMaterial(request.RequestId);
                    List<RequestOtherMaterialsByIssueDTO> requestOtherMaterialsDTOs = _mapper.Map<List<RequestOtherMaterialsByIssueDTO>>(requestMaterials);
                    IssueDetailDTO detail = new IssueDetailDTO()
                    {
                        RequestName = request.RequestName,
                        ApproverName = request.Approver == null ? null : request.Approver.Lastname + " " + request.Approver.Firstname,
                        CableRoutingName = issue.CableRoutingName,
                        Group = issue.Group,
                        RequestCableDTOs = requestCableDTOs,
                        RequestOtherMaterialsDTOs = requestOtherMaterialsDTOs
                    };
                    data.Add(detail);
                }
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListDoing()
        {
            try
            {
                List<Issue> list = _daoIssue.getListIssueStatusDoing();
                List<IssueListDTO> data = _mapper.Map<List<IssueListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPagedAll(string? filter, int page)
        {
            try
            {
                List<Issue> list = _daoIssue.getListIssueAllStatus(filter, page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int rowCount = _daoIssue.getRowCount(filter);
                Pagination<IssueListDTO> data = new Pagination<IssueListDTO>()
                {
                    CurrentPage = page,
                    List = DTOs,
                    RowCount = rowCount
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPagedDoing(int page)
        {
            try
            {
                List<Issue> list = _daoIssue.getListIssueStatusDoing(page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int rowCount = _daoIssue.getRowCount();
                Pagination<IssueListDTO> data = new Pagination<IssueListDTO>()
                {
                    CurrentPage = page,
                    List = DTOs,
                    RowCount = rowCount
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(Guid issueId, IssueUpdateDTO DTO)
        {
            try
            {
                Issue? issue = _daoIssue.getIssue(issueId);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                if (DTO.IssueName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.IssueCode.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (issue.Status == IssueStatus.Done.getDescription())
                {
                    return new ResponseBase(false, "Sự vụ đã được xử lý", (int)HttpStatusCode.Conflict);
                }
                issue.IssueName = DTO.IssueName.Trim();
                issue.IssueCode = DTO.IssueCode.Trim();
                issue.Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim();
                issue.CreatedDate = DTO.CreatedDate;
                issue.CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim();
                issue.Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim();
                issue.Status = DTO.Status;
                issue.UpdateAt = DateTime.Now;
                _daoIssue.UpdateIssue(issue);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
