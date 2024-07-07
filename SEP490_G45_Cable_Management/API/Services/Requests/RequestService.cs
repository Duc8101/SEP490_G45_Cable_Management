using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
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

        public async Task<ResponseBase> Approve(Guid requestId, Guid approverId, string approverName)
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
                if (request.RequestCategoryId == (int) RequestCategories.Export || request.RequestCategoryId == (int)RequestCategories.Deliver 
                    || request.RequestCategoryId == (int)RequestCategories.Cancel_Inside)
                {
                   // return await ApproveRequestExportDeliverCancelInside(approverId, request, approverName);
                }
                if (request.RequestCategoryId == (int)RequestCategories.Recovery)
                {
                   // return await ApproveRequestRecovery(approverId, request, approverName);
                }
                if (request.RequestCategoryId == (int)RequestCategories.Cancel_Outside)
                {
                    //return await ApproveRequestCancelOutside(approverId, request, approverName);
                }
                return new ResponseBase(false, "Không hỗ trợ yêu cầu " + request.RequestCategory.RequestCategoryName, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid creatorId)
        {
            ResponseBase response = RequestHelper.CheckRequestNameAndRequestCategoryValidWhenCreateRequest(DTO.RequestName, DTO.RequestCategoryId, RequestCategories.Cancel_Inside);
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
                // ----------------------- check cable valid --------------------------
                response = RequestHelper.CheckCableValidCreateCancelInside(_daoCable, DTO.CableCancelInsideDTOs);
                // if exist cable invalid
                if (response.Success == false)
                {
                    return response;
                }
                // ----------------------- check material valid --------------------------
                response = RequestHelper.CheckMaterialValidCreateExportDeliverCancelInside(_daoOtherMaterial, DTO.OtherMaterialsCancelInsideDTOs);
                // if exist material invalid
                if (response.Success == false)
                {
                    return response;
                }
                //----------------------------- create request --------------------------------
                Guid requestId = RequestHelper.CreateRequest(_daoRequest, DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, creatorId, DTO.RequestCategoryId, null);
                //----------------------------- create request cable --------------------------------
                if (DTO.CableCancelInsideDTOs.Count > 0)
                {
                    foreach (CableCancelInsideDTO item in DTO.CableCancelInsideDTOs)
                    {
                        Cable? cable = _daoCable.getCable(item.CableId);
                        RequestCable requestCable = _mapper.Map<RequestCable>(cable);
                        requestCable.RequestId = requestId;
                        requestCable.CreatedAt = DateTime.Now;
                        requestCable.UpdateAt = DateTime.Now;
                        requestCable.IsDeleted = false;
                        _daoRequestCable.CreateRequestCable(requestCable);
                    }
                }
                //----------------------------- create request material --------------------------------
                if (DTO.OtherMaterialsCancelInsideDTOs.Count > 0)
                {
                    foreach (OtherMaterialsExportDeliverCancelInsideDTO item in DTO.OtherMaterialsCancelInsideDTOs)
                    {
                        RequestOtherMaterial requestMaterial = _mapper.Map<RequestOtherMaterial>(item);
                        requestMaterial.RequestId = requestId;
                        requestMaterial.CreatedAt = DateTime.Now;
                        requestMaterial.UpdateAt = DateTime.Now;
                        requestMaterial.IsDeleted = false;
                        _daoRequestOtherMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await RequestHelper.sendEmailToAdmin(_daoUser, DTO.RequestName.Trim(), RequestCategories.Cancel_Inside.ToString(), issueCode);
                return new ResponseBase(true, "Tạo yêu cầu thành công");

            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid creatorId)
        {
            ResponseBase response = RequestHelper.CheckRequestNameAndRequestCategoryValidWhenCreateRequest(DTO.RequestName, DTO.RequestCategoryId, RequestCategories.Cancel_Outside);
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
                // ------------------------- create cable ------------------------------
                List<Cable> listCable = new List<Cable>();
                if (DTO.CableCancelOutsideDTOs.Count > 0)
                {
                    foreach (CableCancelOutsideDTO item in DTO.CableCancelOutsideDTOs)
                    {
                        Cable cable = _mapper.Map<Cable>(item);
                        cable.CableId = Guid.NewGuid();
                        cable.CreatorId = creatorId;
                        cable.CreatedAt = DateTime.Now;
                        cable.UpdateAt = DateTime.Now;
                        cable.IsDeleted = false;
                        cable.IsExportedToUse = false;
                        cable.IsInRequest = true;
                        cable.WarehouseId = null;
                        _daoCable.CreateCable(cable);
                        listCable.Add(cable);
                    }
                }
                //-------------------- create other material---------------------------
                Dictionary<int, int> dic = new Dictionary<int, int>();
                if (DTO.OtherMaterialsCancelOutsideDTOs.Count > 0)
                {
                    foreach (OtherMaterialsCancelOutsideDTO item in DTO.OtherMaterialsCancelOutsideDTOs)
                    {
                        OtherMaterial? material = _daoOtherMaterial.getOtherMaterial(item);
                        if (material == null)
                        {
                            material = _mapper.Map<OtherMaterial>(DTO);
                            material.CreatedAt = DateTime.Now;
                            material.UpdateAt = DateTime.Now;
                            material.WarehouseId = null;
                            material.IsDeleted = true;
                            _daoOtherMaterial.CreateOtherMaterial(material);
                            dic[material.OtherMaterialsId] = item.Quantity;
                        }
                        else
                        {
                            dic[material.OtherMaterialsId] = item.Quantity;
                        }
                    }
                }
                // --------------------------- create request ------------------------------------
                Guid requestId = RequestHelper.CreateRequest(_daoRequest, DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, creatorId, DTO.RequestCategoryId, null);
                // --------------------------- create request cable ------------------------------------
                if (listCable.Count > 0)
                {
                    foreach (Cable item in listCable)
                    {
                        RequestCable requestCable = _mapper.Map<RequestCable>(item);
                        requestCable.RequestId = requestId;
                        requestCable.CreatedAt = DateTime.Now;
                        requestCable.UpdateAt = DateTime.Now;
                        requestCable.IsDeleted = false;
                        _daoRequestCable.CreateRequestCable(requestCable);
                    }
                }
                // --------------------------- create request material ------------------------------------
                if (dic.Count > 0)
                {
                    foreach (int otherMaterialId in dic.Keys)
                    {
                        RequestOtherMaterial requestMaterial = new RequestOtherMaterial()
                        {
                            RequestId = requestId,
                            OtherMaterialsId = otherMaterialId,
                            Quantity = dic[otherMaterialId],
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        _daoRequestOtherMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await RequestHelper.sendEmailToAdmin(_daoUser, DTO.RequestName.Trim(), RequestCategories.Cancel_Outside.ToString(), issueCode);
                return new ResponseBase(true, "Tạo yêu cầu thành công");

            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
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

        public async Task<ResponseBase> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid creatorId)
        {
            ResponseBase response = RequestHelper.CheckRequestNameAndRequestCategoryValidWhenCreateRequest(DTO.RequestName, DTO.RequestCategoryId, RequestCategories.Recovery);
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
                // ------------------------------- create cable --------------------------
                List<Cable> listCable = new List<Cable>();
                if (DTO.CableRecoveryDTOs.Count > 0)
                {
                    foreach (CableCreateUpdateDTO item in DTO.CableRecoveryDTOs)
                    {
                        Cable cable = _mapper.Map<Cable>(DTO);
                        cable.CableId = Guid.NewGuid();
                        cable.CreatorId = creatorId;
                        cable.CreatedAt = DateTime.Now;
                        cable.UpdateAt = DateTime.Now;
                        cable.IsDeleted = false;
                        cable.IsExportedToUse = false;
                        cable.IsInRequest = true;
                        _daoCable.CreateCable(cable);
                        listCable.Add(cable);
                    }
                }
                // ------------------------------- create material --------------------------
                // list warehouse
                List<int?> listWarehouseId = new List<int?>();
                // list quantity
                List<int> listQuantity = new List<int>();
                // list material id
                List<int> listOtherMaterialId = new List<int>();
                // list status
                List<string?> listStatus = new List<string?>();
                if (DTO.OtherMaterialsRecoveryDTOs.Count > 0)
                {
                    foreach (OtherMaterialsRecoveryDTO item in DTO.OtherMaterialsRecoveryDTOs)
                    {
                        OtherMaterial? material = _daoOtherMaterial.getOtherMaterial(item);
                        // if material not exist
                        if (material == null)
                        {
                            material = _mapper.Map<OtherMaterial>(DTO);
                            material.Quantity = 0;
                            material.CreatedAt = DateTime.Now;
                            material.UpdateAt = DateTime.Now;
                            material.IsDeleted = false;
                            _daoOtherMaterial.CreateOtherMaterial(material);
                            listWarehouseId.Add(item.WarehouseId);
                            listQuantity.Add(item.Quantity);
                            listOtherMaterialId.Add(material.OtherMaterialsId);
                            listStatus.Add(item.Status.Trim());
                        }
                        else
                        {
                            listWarehouseId.Add(item.WarehouseId);
                            listQuantity.Add(item.Quantity);
                            listOtherMaterialId.Add(material.OtherMaterialsId);
                            listStatus.Add(null);
                            // update material
                            material.UpdateAt = DateTime.Now;
                            _daoOtherMaterial.UpdateOtherMaterial(material);
                        }
                    }
                }
                //----------------------------- create request --------------------------------
                Guid requestId = RequestHelper.CreateRequest(_daoRequest, DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, creatorId, DTO.RequestCategoryId, null);
                //----------------------------- create request cable --------------------------------
                if (listCable.Count > 0)
                {
                    foreach (Cable cable in listCable)
                    {
                        RequestCable request = _mapper.Map<RequestCable>(cable);
                        request.RequestId = requestId;
                        request.CreatedAt = DateTime.Now;
                        request.UpdateAt = DateTime.Now;
                        request.IsDeleted = false;
                        _daoRequestCable.CreateRequestCable(request);
                    }
                }
                //----------------------------- create request material --------------------------------
                if (listOtherMaterialId.Count > 0)
                {
                    for (int i = 0; i < listOtherMaterialId.Count; i++)
                    {
                        RequestOtherMaterial request = new RequestOtherMaterial()
                        {
                            RequestId = requestId,
                            OtherMaterialsId = listOtherMaterialId[i],
                            Quantity = listQuantity[i],
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false,
                            RecoveryDestWarehouseId = listWarehouseId[i],
                            Status = listStatus[i]
                        };
                        _daoRequestOtherMaterial.CreateRequestOtherMaterial(request);
                    }
                }
                // ----------------------------- send email to admin ---------------------------
                await RequestHelper.sendEmailToAdmin(_daoUser, DTO.RequestName.Trim(), RequestCategories.Recovery.ToString(), issueCode);
                return new ResponseBase(true, "Tạo yêu cầu thành công");

            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
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
