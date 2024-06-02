using API.Model.DAO;
using API.Services.IService;
using AutoMapper;
using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
using DataAccess.DTO.RequestDTO;
using DataAccess.Entity;
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

        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedAll(string? filter, int page)
        {
            try
            {
                List<Issue> list = await daoIssue.getListPagedAll(filter, page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int RowCount = await daoIssue.getRowCount(filter);
                PagedResultDTO<IssueListDTO> result = new PagedResultDTO<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedDoing(int page)
        {
            try
            {
                List<Issue> list = await daoIssue.getListPagedDoing(page);
                List<IssueListDTO> DTOs = _mapper.Map<List<IssueListDTO>>(list);
                int RowCount = await daoIssue.getRowCount();
                PagedResultDTO<IssueListDTO> result = new PagedResultDTO<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<IssueListDTO>?>> ListDoing()
        {
            try
            {
                List<Issue> list = await daoIssue.getListDoing();
                List<IssueListDTO> data = _mapper.Map<List<IssueListDTO>>(list);
                return new ResponseDTO<List<IssueListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO, Guid CreatorID)
        {
            if (DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
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
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                if (DTO.IssueName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.IssueCode.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseDTO<bool>(false, "Sự vụ đã được xử lý", (int)HttpStatusCode.Conflict);
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
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                await daoIssue.DeleteIssue(issue);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<IssueDetailDTO>?>> Detail(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<List<IssueDetailDTO>?>(null, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
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
                return new ResponseDTO<List<IssueDetailDTO>?>(details, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<IssueDetailDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
