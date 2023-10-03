using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.Model.DAO;
using DataAccess.Entity;
using DataAccess.Const;

namespace API.Services
{
    public class RequestService
    {
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAORequestCategory daoCategory = new DAORequestCategory();
        private async Task<List<RequestListDTO>> getList(string? name, string? status, int page)
        {
            List<Request> list = await daoRequest.getList(name, status, page);
            List<RequestListDTO> result = new List<RequestListDTO>();
            foreach (Request item in list)
            {
                RequestCategory? category = await daoCategory.getRequestCategory(item.RequestCategoryId);
                if (category != null)
                {
                    RequestListDTO DTO = new RequestListDTO()
                    {
                        RequestId = item.RequestId,
                        RequestName = item.RequestName,
                        Content = item.Content,
                        CreatorId = item.CreatorId,
                        ApproverId = item.ApproverId,
                        Status = item.Status,
                        RequestCategoryName = category.RequestCategoryName
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }
        public async Task<PagedResultDTO<RequestListDTO>> List(string? name, string? status, int page)
        {
            List<RequestListDTO> list = await getList(name, status, page);
            int RowCount = await daoRequest.getRowCount(name, status);
            return new PagedResultDTO<RequestListDTO>(page, RowCount,PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, list);
        }
    }
}
