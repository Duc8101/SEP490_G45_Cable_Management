using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using DataAccess.DTO.CableDTO;
using API.Model.DAO;
using DataAccess.DTO.OtherMaterialsDTO;

namespace API.Services
{
    public class RequestService
    {
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAORequestCable daoRequestCable = new DAORequestCable();
        private readonly DAOCable daoCable = new DAOCable();
        private readonly DAOOtherMaterial daoOtherMaterial = new DAOOtherMaterial();
        private readonly DAORequestOtherMaterial daoRequestMaterial = new DAORequestOtherMaterial();
        private async Task<List<RequestListDTO>> getList(string? name, string? status, Guid? CreatorID, int page)
        {
            List<Request> list = await daoRequest.getList(name, status, CreatorID, page);
            List<RequestListDTO> result = new List<RequestListDTO>();
            foreach (Request item in list)
            {
                RequestListDTO DTO = new RequestListDTO()
                {
                    RequestId = item.RequestId,
                    RequestName = item.RequestName,
                    Content = item.Content,
                    CreatorName = item.Creator.Lastname + " " + item.Creator.Firstname,
                    ApproverName = item.Approver == null ? null : item.Approver.Lastname + " " + item.Approver.Firstname,
                    Status = item.Status,
                    RequestCategoryName = item.RequestCategory.RequestCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, string? status, Guid? CreatorID, int page)
        {
            try
            {
                List<RequestListDTO> list = await getList(name, status, CreatorID, page);
                int RowCount = await daoRequest.getRowCount(name, status, CreatorID);
                PagedResultDTO<RequestListDTO> result = new PagedResultDTO<RequestListDTO>(page, RowCount, PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(result, string.Empty);
            }
            catch(Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        private async Task<ResponseDTO<bool>> isCableValid(List<CableExportDTO> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(CableExportDTO export in list)
            {
                Cable? cable = await daoCable.getCable(export.CableId);
                // if exist cable not found
                if(cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp với ID " + export.CableId, (int) HttpStatusCode.NotFound);
                }
                // if invalid start point, end point or deleted
                if(export.StartPoint < cable.StartPoint || export.EndPoint > cable.EndPoint || export.StartPoint > export.EndPoint || cable.IsDeleted)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + export.CableId + " có chỉ số đầu hoặc chỉ số cuối không hợp lệ hoặc đã bị hủy", (int) HttpStatusCode.NotAcceptable);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + export.CableId + " đã được sử dụng", (int) HttpStatusCode.NotAcceptable);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }

        private async Task<ResponseDTO<bool>> isMaterialValid(List<OtherMaterialsExportDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (OtherMaterialsExportDTO export in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(export.OtherMaterialsId);
                // if not found
                if(material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu vs ID = " + export.OtherMaterialsId, (int) HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if(material.Quantity < export.Quantity)
                {
                    return new ResponseDTO<bool>(false, "Vật liệu vs ID = " + export.OtherMaterialsId + " không có đủ số lượng", (int) HttpStatusCode.NotAcceptable);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private async Task CreateRequestCable(List<CableExportDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (CableExportDTO item in list)
                {
                    RequestCable request = new RequestCable()
                    {
                        RequestId = RequestID,
                        CableId = item.CableId,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.EndPoint - item.StartPoint,
                        RecoveryDestWarehouseId = item.WarehouseId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    await daoRequestCable.CreateRequestCable(request);
                }
            }
        }
        private async Task CreateRequestMaterial(List<OtherMaterialsExportDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach(OtherMaterialsExportDTO item in list)
                {
                    RequestOtherMaterial request = new RequestOtherMaterial()
                    {
                        RequestId = RequestID,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RecoveryDestWarehouseId = item.WarehouseId,
                    };
                    await daoRequestMaterial.CreateRequestOtherMaterial(request);
                }
            }
        }
        private async Task SetCableInRequest(Guid RequestID)
        {
            List<RequestCable> list = await daoRequestCable.getList(RequestID);
            foreach (RequestCable item in list)
            {
                item.Cable.IsInRequest = true;
                await daoCable.UpdateCable(item.Cable);
            }
        }
        public async Task<ResponseDTO<bool>> CreateExport(RequestCreateExportDTO DTO, Guid CreatorID)
        {
            if(DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int) HttpStatusCode.NotAcceptable);
            }
            if(DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_EXPORT)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu xuất kho", (int) HttpStatusCode.NotAcceptable);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
                }
                // if issue approved
                if (issue.Status.Equals(RequestConst.STATUS_APPROVED))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int) HttpStatusCode.NotAcceptable);
                }
                ResponseDTO<bool> response = await isCableValid(DTO.CableExportDTOs);
                // if cable invalid
                if(response.Success == false)
                {
                    return response;
                }
                response = await isMaterialValid(DTO.OtherMaterialsExportDTOs);
                // if material invalid
                if (response.Success == false)
                {
                    return response;
                }
                Request request = new Request()
                {
                    RequestId = Guid.NewGuid(),
                    RequestName = DTO.RequestName.Trim(),
                    Content = DTO.Content == null || DTO.Content.Trim().Length == 0 ? null : DTO.Content.Trim(),
                    IssueId = DTO.IssueId,
                    CreatorId = CreatorID,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT,
                    Status = RequestConst.STATUS_PENDING,
                };
                await daoRequest.CreateRequest(request);
                await CreateRequestCable(DTO.CableExportDTOs, request.RequestId);
                await CreateRequestMaterial(DTO.OtherMaterialsExportDTOs, request.RequestId);
                //await SetCableInRequest(request.RequestId);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }  
        }
    }
}
