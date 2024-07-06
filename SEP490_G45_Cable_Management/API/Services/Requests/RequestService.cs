using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.RequestDTO;
using Common.Entity;
using Common.Enum;
using Common.Paginations;
using DataAccess.DAO;
using DataAccess.Helper;
using System.Net;

namespace API.Services.Requests
{
    public class RequestService : BaseService, IRequestService
    {
        private readonly DAORequest _daoRequest;
        private readonly DAOIssue _daoIssue;
        private readonly DAOCable _daoCable;
        private readonly DAOOtherMaterial _daoOtherMaterial;
        private readonly DAORequestCable _daoRequestCable;
        private readonly DAORequestOtherMaterial _daoRequestOtherMaterial;
        private readonly DAOUser _daoUser;
        private readonly DAOWarehouse _daoWarehouse;
        public RequestService(IMapper mapper, DAORequest daoRequest, DAOIssue daoIssue, DAOCable daoCable
            , DAOOtherMaterial daoOtherMaterial, DAORequestCable daoRequestCable, DAORequestOtherMaterial daoRequestOtherMaterial
            , DAOUser daoUser, DAOWarehouse daoWarehouse) : base(mapper)
        {
            _daoRequest = daoRequest;
            _daoIssue = daoIssue;
            _daoCable = daoCable;
            _daoOtherMaterial = daoOtherMaterial;
            _daoRequestCable = daoRequestCable;
            _daoRequestOtherMaterial = daoRequestOtherMaterial;
            _daoUser = daoUser;
            _daoWarehouse = daoWarehouse;
        }

        public Task<ResponseBase> Approve(Guid requestId, Guid approverId, string approverName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBase> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid creatorId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBase> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid creatorId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBase> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid creatorId)
        {
            ResponseBase response = RequestHelper.CheckRequestNameAndRequestCategoryValidWhenCreateRequest(DTO.RequestName, DTO.RequestCategoryId, RequestCategories.Deliver);
            if (response.Success == false)
            {
                return response;
            }

            try
            {
                Warehouse? ware = _daoWarehouse.getWarehouse(DTO.DeliverWareHouseId);
                if (ware == null)
                {
                    return new ResponseBase(false, "Không tìm thấy kho nhận", (int)HttpStatusCode.NotFound);
                }
                response = RequestHelper.CheckCableValidWhenCreateRequestExportDeliver(_daoCable, DTO.CableDeliverDTOs);
                if (response.Success == false)
                {
                    return response;
                }
                response = RequestHelper.CheckMaterialValidCreateExportDeliverCancelInside(_daoOtherMaterial, DTO.OtherMaterialsDeliverDTOs);
                if (response.Success == false)
                {
                    return response;
                }
                //----------------------------- create request --------------------------------
                Guid requestId = RequestHelper.CreateRequest(_daoRequest, DTO.RequestName.Trim(), DTO.Content, null, creatorId, DTO.RequestCategoryId, null);
                RequestHelper.CreateRequestCableExportDeliver(_daoRequestCable, DTO.CableDeliverDTOs, requestId);
                RequestHelper.CreateRequestMaterialExportDeliver(_daoRequestOtherMaterial, DTO.OtherMaterialsDeliverDTOs, requestId);
                // ----------------------------- send email to admin ---------------------------
                await RequestHelper.sendEmailToAdmin(_daoUser, DTO.RequestName.Trim(), RequestCategories.Deliver.ToString(), null);
                return new ResponseBase(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ResponseBase> CreateRequestExport(RequestCreateExportDTO DTO, Guid creatorId)
        {
            ResponseBase response = RequestHelper.CheckRequestNameAndRequestCategoryValidWhenCreateRequest(DTO.RequestName, DTO.RequestCategoryId, RequestCategories.Export);
            if (response.Success == false)
            {
                return response;
            }
            try
            {
                response = RequestHelper.CheckIssueValidWhenCreateRequest(_daoIssue, DTO.IssueId);
                if (response.Success == false)
                {
                    return response;
                }
                string issueCode = response.Message;
                response = RequestHelper.CheckCableValidWhenCreateRequestExportDeliver(_daoCable, DTO.CableExportDTOs);
                if (response.Success == false)
                {
                    return response;
                }
                response = RequestHelper.CheckMaterialValidCreateExportDeliverCancelInside(_daoOtherMaterial, DTO.OtherMaterialsExportDTOs);
                if (response.Success == false)
                {
                    return response;
                }
                //----------------------------- create request --------------------------------
                Guid requestId = RequestHelper.CreateRequest(_daoRequest, DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, creatorId, DTO.RequestCategoryId, null);
                RequestHelper.CreateRequestCableExportDeliver(_daoRequestCable, DTO.CableExportDTOs, requestId);
                RequestHelper.CreateRequestMaterialExportDeliver(_daoRequestOtherMaterial, DTO.OtherMaterialsExportDTOs, requestId);
                // ----------------------------- send email to admin ---------------------------
                await RequestHelper.sendEmailToAdmin(_daoUser, DTO.RequestName.Trim(), RequestCategories.Export.ToString(), issueCode);
                return new ResponseBase(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public Task<ResponseBase> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid creatorId)
        {
            throw new NotImplementedException();
        }

        public ResponseBase Delete(Guid requestId)
        {
            try
            {
                Request? request = _daoRequest.getRequest(requestId);
                if (request == null)
                {
                    return new ResponseBase(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (!request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseBase(false, "Yêu cầu đã được chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                _daoRequest.DeleteRequest(request);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Detail(Guid requestId)
        {
            try
            {
                Request? request = _daoRequest.getRequest(requestId);
                if (request == null)
                {
                    return new ResponseBase("Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                List<RequestCable> requestCables = _daoRequestCable.getListRequestCable(requestId);
                List<RequestCableListDTO> requestCableDTOs = _mapper.Map<List<RequestCableListDTO>>(requestCables);
                List<RequestOtherMaterial> requestMaterials = _daoRequestOtherMaterial.getListRequestOtherMaterial(requestId);
                List<RequestOtherMaterialsListDTO> RequestOtherMaterialsDTOs = _mapper.Map<List<RequestOtherMaterialsListDTO>>(requestMaterials);
                RequestDetailDTO data = _mapper.Map<RequestDetailDTO>(request);
                data.RequestCableDTOs = requestCableDTOs;
                data.RequestOtherMaterialsDTOs = RequestOtherMaterialsDTOs;
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase List(string? name, int? requestCategoryId, string? status, Guid? creatorId, int page)
        {
            try
            {
                List<Request> list = _daoRequest.getListRequest(name, requestCategoryId, status, creatorId, page);
                List<RequestListDTO> DTOs = _mapper.Map<List<RequestListDTO>>(list);
                int rowCount = _daoRequest.getRowCount(name, requestCategoryId, status, creatorId);
                Pagination<RequestListDTO> data = new Pagination<RequestListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    List = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Reject(Guid requestId, Guid rejectorId)
        {
            try
            {
                Request? request = _daoRequest.getRequest(requestId);
                if (request == null)
                {
                    return new ResponseBase(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (request.Status != RequestConst.STATUS_PENDING)
                {
                    return new ResponseBase(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                // ------------------- update request ---------------
                RequestHelper.UpdateRequest(_daoRequest, request, rejectorId, RequestConst.STATUS_REJECTED);
                return new ResponseBase(true);
            }
            catch (Exception ex)
            {
                return new ResponseBase(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
