using API.Services.IService;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.IssueDTO;
using Common.DTO.RequestDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Service
{
    public class IssueService : BaseService, IIssueService
    {
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAORequestCable daoRequestCable = new DAORequestCable();
        private readonly DAORequestOtherMaterial daoRequestMaterial = new DAORequestOtherMaterial();

        public IssueService(IMapper mapper) : base(mapper)
        {

        }

        public async Task<ResponseBase<Pagination<IssueListDTO>?>> ListPagedAll(string? filter, int page)
        {
            try
            {
                List<Issue> list = await daoIssue.getListPagedAll(filter, page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int RowCount = await daoIssue.getRowCount(filter);
                Pagination<IssueListDTO> result = new Pagination<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<Pagination<IssueListDTO>?>> ListPagedDoing(int page)
        {
            try
            {
                List<Issue> list = await daoIssue.getListPagedDoing(page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int RowCount = await daoIssue.getRowCount();
                Pagination<IssueListDTO> result = new Pagination<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<IssueListDTO>?>> ListDoing()
        {
            try
            {
                List<Issue> list = await daoIssue.getListDoing();
                List<IssueListDTO> data = _mapper.Map<List<IssueListDTO>>(list);
                return new ResponseBase<List<IssueListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(IssueCreateDTO DTO, Guid CreatorID)
        {
            if (DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }
            Issue issue = _mapper.Map<Issue>(DTO);
            issue.IssueId = Guid.NewGuid();
            issue.CreatedAt = DateTime.Now;
            issue.UpdateAt = DateTime.Now;
            issue.CreatorId = CreatorID;
            issue.Status = IssueConst.STATUS_DOING;
            issue.IsDeleted = false;
            try
            {
                await daoIssue.CreateIssue(issue);
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                if (DTO.IssueName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.IssueCode.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseBase<bool>(false, "Sự vụ đã được xử lý", (int)HttpStatusCode.Conflict);
                }
                issue.IssueName = DTO.IssueName.Trim();
                issue.IssueCode = DTO.IssueCode.Trim();
                issue.Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim();
                issue.CreatedDate = DTO.CreatedDate;
                issue.CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim();
                issue.Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim();
                issue.Status = DTO.Status;
                issue.UpdateAt = DateTime.Now;
                await daoIssue.UpdateIssue(issue);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                await daoIssue.DeleteIssue(issue);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<IssueDetailDTO>?>> Detail(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseBase<List<IssueDetailDTO>?>(null, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                List<Request> requests = await daoRequest.getListByIssue(IssueID);
                List<IssueDetailDTO> details = new List<IssueDetailDTO>();
                foreach (Request request in requests)
                {
                    List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
                    List<RequestCableByIssueDTO> requestCableDTOs = _mapper.Map<List<RequestCableByIssueDTO>>(requestCables);
                    List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
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
                    details.Add(detail);
                }
                return new ResponseBase<List<IssueDetailDTO>?>(details, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<IssueDetailDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
