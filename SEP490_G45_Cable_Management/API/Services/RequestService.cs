using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using API.Model.DAO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using DataAccess.DTO.CableDTO;

namespace API.Services
{
    public class RequestService
    {
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAORequestCable daoRequestCable = new DAORequestCable();
        private readonly DAOCable daoCable = new DAOCable();
        private async Task<List<RequestListDTO>> getList(string? name, string? status, int page)
        {
            List<Request> list = await daoRequest.getList(name, status, page);
            List<RequestListDTO> result = new List<RequestListDTO>();
            foreach (Request item in list)
            {
                RequestListDTO DTO = new RequestListDTO()
                {
                    RequestId = item.RequestId,
                    RequestName = item.RequestName,
                    Content = item.Content,
                    CreatorId = item.CreatorId,
                    ApproverId = item.ApproverId,
                    Status = item.Status,
                    RequestCategoryName = item.RequestCategory.RequestCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<RequestListDTO>> List(string? name, string? status, int page)
        {
            List<RequestListDTO> list = await getList(name, status, page);
            int RowCount = await daoRequest.getRowCount(name, status);
            return new PagedResultDTO<RequestListDTO>(page, RowCount,PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, list);
        }

        public async Task<ResponseDTO<bool>> CreateExport(RequestCreateExportDTO DTO, Guid CreatorID)
        {
            Issue? issue = await daoIssue.getIssue(DTO.IssueId);
            // if not found issue
            if (issue == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
            }
            // if issue approved
            if (issue.Status != null && issue.Status.Equals(RequestConst.STATUS_APPROVED))
            {
                return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int) HttpStatusCode.NotAcceptable);
            }
            return new ResponseDTO<bool>(true,"");
        }
        /*public async Task<ResponseDTO<bool>> Create(RequestCreateDTO DTO, Guid CreatorID)
        {
            // if not request deliver warehouse
            if(DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_DELIVER)
            {
                // if empty issue
                if(DTO.IssueId == null)
                {
                    return new ResponseDTO<bool>(false, "Không được để trống sự vụ", (int) HttpStatusCode.NotAcceptable);
                }
                Issue? issue = await daoIssue.getIssue(DTO.IssueId.Value);
                // if not found issue
                if(issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
                }
                // if issue approved
                if (issue.Status.Equals(RequestConst.STATUS_APPROVED))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int) HttpStatusCode.NotAcceptable);
                }
            }
            // ------------------------- create request ----------------------------
            Request request = new Request()
            {
                RequestId = Guid.NewGuid(),
                RequestName = DTO.RequestName.Trim(),
                Content = DTO.Content == null || DTO.Content.Trim().Length == 0 ? null : DTO.Content.Trim(),
                CreatorId = CreatorID,
                IssueId = DTO.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER ? null : DTO.IssueId,
                Status = RequestConst.STATUS_PENDING,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                RequestCategoryId = DTO.RequestCategoryId,
                DeliverWarehouseId = DTO.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER ? DTO.DeliverWarehouseId : null
            };
            int number = await daoRequest.CreateRequest(request);
            if (number == 0)
            {
                return new ResponseDTO<bool>(false, "Tạo yêu cầu thất bại", (int) HttpStatusCode.Conflict);
            }
            // ------------------------- create request cable ----------------------------
            number = 0;
            // if import cable
            if(DTO.CableImportDTOs.Count != 0)
            {
                foreach(CableImportDTO import in DTO.CableImportDTOs)
                {
                    Cable? cable = await daoCable.getCable(import.CableId);
                    RequestCable requestCable = new RequestCable()
                    {
                        RequestId = request.RequestId,
                    };
                }
            }      
        }*/
    }
}
